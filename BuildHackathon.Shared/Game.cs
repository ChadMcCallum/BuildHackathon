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

        public int TotalPlayers
        {
            get { return this.RedTeam.Players.Count + this.BlueTeam.Players.Count; }
        }

        public Game()
        {
            this.ID = Guid.NewGuid();

            this.QRCodeImage = string.Format("http://chart.apis.google.com/chart?cht=qr&chs=300x300&chl={0}&chld=H|0)",
                                             Uri.EscapeDataString(
                                                 "http://buildhackathon.cloudapp.net/client.html?id=" +
                                                 this.ID.ToString()));
            BlueTeam = new Team() { Name = "Blue" };
            RedTeam = new Team() {Name = "Red"};
        }

        public void AddPlayerToTeam(Player player)
        {
            if (BlueTeam.Players.Count > RedTeam.Players.Count)
            {
                BlueTeam.AddPlayer(player);
            }
            else
            {
                RedTeam.AddPlayer(player);
            }
        }
    }
}
