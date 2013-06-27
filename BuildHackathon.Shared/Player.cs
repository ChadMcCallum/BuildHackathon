using Newtonsoft.Json;

namespace BuildHackathon.Shared
{
    public class Player
    {
        public Player()
        {
            
        }

        public Player(string connectionId)
        {
            this.ConnectionID = connectionId;
        }

        public string ImageURL { get; set; }

        public string Name { get; set; }

        public string ConnectionID { get; set; }
        
        [JsonIgnore]
        public Team Team { get; set; }
        [JsonIgnore]
        public Game Game { get; set; }

		public int Score { get; set; }
    }
}