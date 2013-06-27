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
	}
}
