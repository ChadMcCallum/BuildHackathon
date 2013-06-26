using System;
using System.Collections.Generic;
using System.Linq;
using BuildHackathon.Shared;
using Microsoft.AspNet.SignalR;

namespace BuildHackathon.Hubs
{
    public class GameHub : Hub
    {
        private static List<GameThread> _games;
        private static List<Player> _players; 

        public GameHub()
        {
            if (_games == null)
            {
                _games = new List<GameThread>();
            }
            if (_players == null)
            {
                _players = new List<Player>();
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected()
        {
            var player = _players.FirstOrDefault(p => p.ConnectionID == Context.ConnectionId);
            if (player != null)
            {
                player.Team.RemovePlayer(player);
            }
            return base.OnDisconnected();
        }

        //host methods
        public Game CreateGame()
        {
            var game = new Game();
            _games.Add(new GameThread(GlobalHost.ConnectionManager.GetHubContext<GameHub>()));

            return game;
        }

        //client methods
        public string JoinGame(Guid id, string name)
        {
            var game = _games.FirstOrDefault(g => g.Game.ID == id);
            if (game == null)
            {
                var gamestate = CreateGame();
                gamestate.ID = id;
                game = _games.First();
            }
            if (game != null)
            {
                var player = new Player(Context.ConnectionId) { Name = name };
                game.AddPlayer(player);
                if (game.Game.TotalPlayers > 2) game.Start();
                return player.Team.Name;
            }
            throw new Exception("Game doesn't exist");
        }


    }
}