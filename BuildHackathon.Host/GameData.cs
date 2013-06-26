using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildHackathon.Shared;

namespace BuildHackathon.Host
{
	public static class GameData
	{
		public static Game Game { get; set; }

		public static event EventHandler OnNewTweet = delegate { };
		public static event EventHandler OnPlayerConnected = delegate { };
		public static event EventHandler OnPlayerDisconnected = delegate { };

		public static event EventHandler OnGameDataUpdated = delegate { };
		public static void UpdateGameData() { OnGameDataUpdated(null, EventArgs.Empty); }
	}
}
