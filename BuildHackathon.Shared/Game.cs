using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildHackathon.Shared
{
    public class Game
    {
        public Guid ID { get; set; }
        public string QRCodeImage { get; set; }
        public Team BlueTeam { get; set; }
        public Team RedTeam { get; set; }
        public List<Guess> Guesses { get; set; }
        public List<Player> Celebrities { get; set; } 

        public int TotalPlayers
        {
            get { return this.RedTeam.Players.Count + this.BlueTeam.Players.Count; }
        }

        public Game(Guid id)
        {
            this.ID = id;
            this.QRCodeImage = string.Format("http://chart.apis.google.com/chart?cht=qr&chs=300x300&chl={0}&chld=H|0)",
                                 Uri.EscapeDataString(
                                     "http://buildhackathon.cloudapp.net/client.html?id=" +
                                     this.ID.ToString()));
            BlueTeam = new Team() { Name = "Blue" };
            RedTeam = new Team() { Name = "Red" };
            
            Celebrities = new List<Player>
                {
                    new Player { Name = "DrunkHulk" },
                    new Player { Name = "DepressedDarth" },
                    new Player { Name = "ShitMyDadSays" },
                    new Player { Name = "shanselman" },
                    new Player { Name = "scottgu" },
                    new Player { Name = "codinghorror" }
                };
        }

        public void AddPlayerToTeam(Player player)
        {
            if (BlueTeam.Players.Count > RedTeam.Players.Count)
            {
                RedTeam.AddPlayer(player);
            }
            else
            {
                BlueTeam.AddPlayer(player);
            }
        }

        public void MakeGuess(Player player, string name)
        {
            this.Guesses.Add(new Guess() { Player = player, Name = name});
        }

        public void SetQuestion(Question question)
        {
            this.Question = question;
            this.Guesses = new List<Guess>();
        }

        public Question Question { get; set; }

    }
}
