using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BuildHackathon.Shared;
using Microsoft.AspNet.SignalR;
using TweetSharp;

namespace BuildHackathon.Hubs
{
    public class GameThread
    {
        private IHubContext _hub;
        private TwitterService _service;
        private Timer _currentQuestionTimer;
        private Random _random;

        public GameThread(IHubContext context)
        {
            _random = new Random();
            IsStarted = false;
            _hub = context;
            this.Game = new Game(Guid.NewGuid().ToString());
            _service = new TwitterService("vCBYBvYAdRrgWs5z0zmD1A", "eHqhut7IVR4aUWlgBwuURl3QssL7ASf7hNIi3AovjY");
            _service.AuthenticateWith("68329856-dHYH7dgh85Qkiv7vaNjoScNDTngJtNrjdH8JCLcvt", "KFtWesqC94jxkgSVYrpMlg4IKIcitmJF7MQW0b5Q");
        }

        public void InitCelebreties()
        {
            foreach (var celeb in Game.Celebrities)
            {
                var user = _service.SearchForUser(new SearchForUserOptions { Q = celeb.Name });
                if (user != null)
                {
                    celeb.ImageURL = user.First().ProfileImageUrl;
                }
            }
        }

        public Game Game { get; set; }

        public bool IsStarted { get; set; }

        public string Host { get; set; }

        public void Start()
        {
            IsStarted = true;
            GetNewQuestion(null);
        }

        private void GetNewQuestion(object obj)
        {
            var options = GetPlayerOptions();
            Player user = null;
            IEnumerable<TwitterStatus> tweets = null;
            while (user == null)
            {
                user = options.OrderBy(x => _random.Next()).First();
                tweets = _service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions
                {
                    ScreenName = user.Name,
                    ExcludeReplies = true,
                    IncludeRts = false
                });
                if (!tweets.Any())
                    user = null;
            }
            var randomIndex = _random.Next(tweets.Count());
            var randomTweet = tweets.ElementAt(randomIndex);
            var question = new Question
                {
                    Tweet = randomTweet.TextDecoded,
                    RightAnswer = randomTweet.User.ScreenName,
                    PlayerOptions = options.ToArray()
                };
            this.Game.SetQuestion(question);
            _hub.Clients.Group(Game.ID).NewQuestion(question);
            _hub.Clients.Client(Host).NewQuestion(question);
            //have 15 seconds to answer
            this._currentQuestionTimer = new Timer(QuestionTimeout, null, 20000, Timeout.Infinite);
        }

        private List<Player> GetPlayerOptions()
        {
            var list = new List<Player>();
            if (this.Game.Type == GameType.CelebsOnly)
            {
                list.AddRange(Game.Celebrities.OrderBy(x => _random.Next()).Take(6));
            }
            else if (this.Game.Type == GameType.PlayersOnly)
            {
                list.AddRange(Game.RedTeam.Players.Concat(Game.BlueTeam.Players).OrderBy(x => _random.Next()).Take(6));
            }
            else
            {
                list.AddRange(
                    Game.Celebrities.Concat(Game.BlueTeam.Players)
                        .Concat(Game.RedTeam.Players)
                        .OrderBy(x => _random.Next())
                        .Take(6));
            }
            return list;
        }

        private void QuestionTimeout(object obj)
        {
            //for each player in game
            var message = string.Format("The right answer was {0}", this.Game.Question.RightAnswer);
            foreach (var player in this.Game.RedTeam.Players.Concat(this.Game.BlueTeam.Players))
            {
                var playerGuess = this.Game.Guesses.FirstOrDefault(p => p.Player.ConnectionID == player.ConnectionID);
                if (playerGuess != null && playerGuess.Name.ToLower() == this.Game.Question.RightAnswer.ToLower())
                {
                    player.Team.Score += 100;
                    player.Score += 100;
                }
                else if (playerGuess != null)
                {
                    player.Team.Score -= 10;
                    player.Score -= 10;
                }
                else
                {
                    player.Team.Score -= 5;
                    player.Score -= 5;
                }
                if (player.Team.Score < 0)
                {
                    player.Team.Score = 0;
                }
                if (player.Score < 0)
                {
                    player.Score = 0;
                }
            }
            var rightPlayer = this.Game.Question.PlayerOptions.Single(p => p.Name.ToLower() == this.Game.Question.RightAnswer.ToLower());
            foreach (var player in this.Game.RedTeam.Players.Concat(this.Game.BlueTeam.Players))
            {
                var playerGuess = this.Game.Guesses.FirstOrDefault(p => p.Player.ConnectionID == player.ConnectionID);
                var result = new {MyScore = player.Score, TeamScore = player.Team.Score, Actual = rightPlayer};
                if (playerGuess == null)
                {
                    //timeout
                    _hub.Clients.Client(player.ConnectionID).Timeout(result);
                }
                else if (playerGuess.Name.ToLower() != this.Game.Question.RightAnswer.ToLower())
                {
                    //wrong
                    _hub.Clients.Client(player.ConnectionID).Wrong(result);
                }
                else
                {
                    //right
                    _hub.Clients.Client(player.ConnectionID).Right(result);
                }
            }
            //update scores
            _hub.Clients.Client(Host).UpdateScore(new [] {Game.RedTeam, Game.BlueTeam});

            //set timer to next question
            _currentQuestionTimer = new Timer(GetNewQuestion, null, 5000, Timeout.Infinite);
        }

        public void AddPlayer(Player player)
        {
            var user = _service.GetUserProfileFor(new GetUserProfileForOptions { ScreenName = player.Name });
            if (user != null)
            {
                player.Name = user.ScreenName;
                player.ImageURL = user.ProfileImageUrl;
                player.Game = this.Game;
                this.Game.AddPlayerToTeam(player);
                _hub.Groups.Add(player.ConnectionID, Game.ID);
                _hub.Clients.Client(Host).NewPlayer(new [] { Game.RedTeam, Game.BlueTeam });
            }
            else
            {
                throw new Exception("User not found on twitter");
            }
        }

        public void Guess(Player player, string name)
        {
            this.Game.MakeGuess(player, name);
            if (this.Game.TotalPlayers == this.Game.Guesses.Count())
            {
                this._currentQuestionTimer.Change(500, Timeout.Infinite);
            }
        }

        public void EndGame()
        {
            _hub.Clients.Group(this.Game.ID).EndGame("Not enough players");
            _hub.Clients.Client(Host).EndGame("More players needed");
            if (this._currentQuestionTimer != null)
            {
                this._currentQuestionTimer.Change(Timeout.Infinite, Timeout.Infinite);
                this._currentQuestionTimer = null;
            }
            this.IsStarted = false;
        }

        public void RemovePlayer(Player player)
        {
            _hub.Clients.Client(Host).RemovePlayer(new[] { this.Game.RedTeam, this.Game.BlueTeam});
        }

        public void CancelGame()
        {
            _hub.Clients.Group(Game.ID).EndGame("Host app closed");
            if (this._currentQuestionTimer != null)
            {
                this._currentQuestionTimer.Change(Timeout.Infinite, Timeout.Infinite);
                this._currentQuestionTimer = null;
            }
            this.IsStarted = false;
        }
    }
}