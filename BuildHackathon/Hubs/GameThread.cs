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

            InitCelebreties();
        }

        private void InitCelebreties()
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

        public void Start()
        {
            IsStarted = true;
            GetNewQuestion(null);
        }

        private void GetNewQuestion(object obj)
        {
            var user = GetRandomUser();
            var tweets = _service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions
                {
                    ScreenName = user.Name,
                    ExcludeReplies = true,
                    IncludeRts = false
                });
            var randomIndex = _random.Next(tweets.Count());
            var randomTweet = tweets.ElementAt(randomIndex);
            var question = new Question
                {
                    Tweet = randomTweet.TextDecoded,
                    RightAnswer = randomTweet.User.ScreenName
                };
            SetPlayerOptions(question, user);
            this.Game.SetQuestion(question);
            _hub.Clients.Group(Game.ID).NewQuestion(question);
            //have 15 seconds to answer
            this._currentQuestionTimer = new Timer(QuestionTimeout, null, 15000, Timeout.Infinite);
        }

        private bool useCelebrity = true;
        private Player GetRandomUser()
        {
            if (useCelebrity)
            {
                useCelebrity = false;
                return Game.Celebrities.OrderBy(x => _random.Next()).First();
            }
            else
            {
                useCelebrity = true;
                return Game.RedTeam.Players.Concat(Game.BlueTeam.Players).OrderBy(x => _random.Next()).First();
            }
        }

        private void SetPlayerOptions(Question question, Player user)
        {
            var list = new List<Player>();
            var celebCount = 6 - Game.TotalPlayers;
            if (celebCount < 3) celebCount = 3;
            list.AddRange(Game.Celebrities.OrderBy(x => _random.Next()).Take(celebCount));
            list.AddRange(Game.RedTeam.Players.Concat(Game.BlueTeam.Players).OrderBy(x => _random.Next()).Take(6 - celebCount));
            if (list.All(p => p.Name != user.Name))
                SetPlayerOptions(question, user);
            else
            {
                question.PlayerOptions = list.OrderBy(o => _random.Next()).ToArray();
            }
        }

        private void QuestionTimeout(object obj)
        {
            //for each player in game
            var message = string.Format("The right answer was {0}", this.Game.Question.RightAnswer);
            foreach (var player in this.Game.RedTeam.Players.Concat(this.Game.BlueTeam.Players))
            {
                var playerGuess = this.Game.Guesses.FirstOrDefault(p => p.Player.ConnectionID == player.ConnectionID);
                if (playerGuess == null)
                {
                    //timeout
                    _hub.Clients.Client(player.ConnectionID).Timeout(message);
                }
                else if (playerGuess.Name != this.Game.Question.RightAnswer)
                {
                    //wrong
                    _hub.Clients.Client(player.ConnectionID).Wrong(message);
                }
                else
                {
                    //right
                    _hub.Clients.Client(player.ConnectionID).Right(message);
                }
            }

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
            if (this._currentQuestionTimer != null)
            {
                this._currentQuestionTimer.Change(Timeout.Infinite, Timeout.Infinite);
                this._currentQuestionTimer = null;
            }
            this.IsStarted = false;
        }
    }
}