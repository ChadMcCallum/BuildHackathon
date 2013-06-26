using System.Collections.Generic;

namespace BuildHackathon.Shared
{
    public class Team
    {
        public string Name { get; set; }
        public List<Player> Players { get; set; }

        public Team()
        {
            Players = new List<Player>();
        }

        public void AddPlayer(Player player)
        {
            this.Players.Add(player);
            player.Team = this;
        }

        public void RemovePlayer(Player player)
        {
            this.Players.Remove(player);
        }
    }
}