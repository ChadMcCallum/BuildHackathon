using System.Collections.Generic;
using BuildHackathon.Shared;
using Microsoft.AspNet.SignalR;

namespace BuildHackathon.Hubs
{
    public class GameHub : Hub
    {
        private static List<Game> _games;

        public GameHub()
        {
            if (_games == null)
            {
                _games = new List<Game>();
            }
        }

        public Game CreateGame()
        {
            var game = new Game();
            _games.Add(game);

            return game;
        }
    }
}