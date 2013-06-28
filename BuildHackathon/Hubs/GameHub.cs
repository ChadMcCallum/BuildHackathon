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
                if(player.Team != null)
                    player.Team.RemovePlayer(player);
                var thread = _games.FirstOrDefault(p => p.Game.ID == player.Game.ID);
                if (thread != null)
                {
                    thread.RemovePlayer(player);
                    if (thread.Game.TotalPlayers < 2)
                    {
                        thread.EndGame();
                    }
                }
            }
            return base.OnDisconnected();
        }

        //host methods
        public Game CreateGame(int type, string[] celebs)
        {
            var thread = new GameThread(GlobalHost.ConnectionManager.GetHubContext<GameHub>());
            _games.Add(thread);
            thread.Game.Type = (GameType)type;
            if (thread.Game.Type != GameType.PlayersOnly)
            {
                thread.Game.SetCelebs(celebs);
                thread.InitCelebreties();
            }
            thread.Host = Context.ConnectionId;

            return thread.Game;
        }

        public void CancelGame()
        {
            var id = Context.ConnectionId;
            var gamethread = _games.FirstOrDefault(g => g.Host == id);
            gamethread.CancelGame();
        }

        //client methods
        public string JoinGame(string id, string name)
        {
            var game = _games.FirstOrDefault(g => g.Game.ID == id);
            //take this out later
            if (game == null)
            {
                var gamestate = CreateGame(3,
                                           new[]
                                               {
                                                   "shanselman", "scottgu", "codinghorror", "billgates", "gblock",
                                                   "mkristensen"
                                               });
                gamestate.ID = id;
                game = _games.First();
            }
            if (game != null)
            {
                var player = new Player(Context.ConnectionId) { Name = name };
                _players.Add(player);
                game.AddPlayer(player);
                if (game.Game.TotalPlayers >= 2 && !game.IsStarted)
                    game.Start();
                else if(!game.IsStarted)
                {
                    Clients.Group(game.Game.ID).Wait();
                    Clients.Client(Context.ConnectionId).Wait();
                }
                else if (game.IsStarted)
                {
                    Clients.Client(Context.ConnectionId).Waiting();
                }
                return player.Team.Name;
            }
            throw new Exception("Game doesn't exist");
        }

        public void Guess(string name)
        {
            var player = _players.FirstOrDefault(p => p.ConnectionID == Context.ConnectionId);
            var gameThread = _games.FirstOrDefault(g => g.Game == player.Game);
            gameThread.Guess(player, name);
        }
    }
}