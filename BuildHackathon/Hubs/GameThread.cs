using System;
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

        public GameThread(IHubContext context)
        {
            _hub = context;
            this.Game = new Game();
            _service = new TwitterService("vCBYBvYAdRrgWs5z0zmD1A", "eHqhut7IVR4aUWlgBwuURl3QssL7ASf7hNIi3AovjY");
            _service.AuthenticateWith("68329856-dHYH7dgh85Qkiv7vaNjoScNDTngJtNrjdH8JCLcvt", "KFtWesqC94jxkgSVYrpMlg4IKIcitmJF7MQW0b5Q");
        }

        public Game Game { get; set; }

        public void Start()
        {
            //send a new question
            GetNewQuestion();
            //set a timer for timeout
            //tell everyone result
            //set a timer for next question
        }

        private void GetNewQuestion()
        {
            var tweets = _service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions
                {
                    ScreenName = "ChadEmm",
                    ExcludeReplies = true,
                    IncludeRts = false
                });
            var randomIndex = new Random().Next(tweets.Count());
            var randomTweet = tweets.ElementAt(randomIndex);
            var question = new Question
                {
                    Tweet = randomTweet.TextDecoded,
                    PlayerOptions = new[]
                        {
                            new Player("") { ImageURL = randomTweet.User.ProfileImageUrl, Name = randomTweet.User.ScreenName }
                        }
                };
            var timer = new Timer(this.SendQuestion, question, 2000, Timeout.Infinite);
        }

        private void SendQuestion(object obj)
        {
            _hub.Clients.All.NewQuestion(obj as Question);
        }

        public void AddPlayer(Player player)
        {
            var user = _service.GetUserProfileFor(new GetUserProfileForOptions {ScreenName = player.Name});
            if (user != null)
            {
                player.Name = user.ScreenName;
                player.ImageURL = user.ProfileImageUrl;
                this.Game.AddPlayerToTeam(player);
            }
            else
            {
                throw new Exception("User not found on twitter");
            }
        }
    }
}