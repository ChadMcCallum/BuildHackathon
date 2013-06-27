using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildHackathon.Shared;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace BuildHackathon.Host
{
	public static class GameData
	{
		public static Game Game { get; set; }
		public static IHubProxy HubProxy { get; set; }
		
		public static List<string> CustomTwitterHandles { get; set; }
		public static GameType? GameType { get; set; }

		private static bool _isTestingOffline = false;
		public static bool IsTestingOffline 
		{ 
			get { return _isTestingOffline; }
			set
			{
				_isTestingOffline = value;
				if (_isTestingOffline)
				{
					Game = new Game("1");
					HubProxy = new HubProxy(new HubConnection(string.Empty), string.Empty);
				}
			}
		}
	}
}
