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

		public event EventHandler OnNewTweet = delegate { };
		public event EventHandler OnPlayerConnected = delegate { };
		public event EventHandler OnPlayerDisconnected = delegate { };
	}
}
