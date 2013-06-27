using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildHackathon.Shared;
using Windows.UI.Xaml.Media.Imaging;

namespace BuildHackathon.Host
{
	public class ClientPlayer
	{
		public Player Player { get; private set; }
		
		public ClientPlayer(Player player)
		{ 
			this.Player = player;
		}

		public BitmapImage Image { get { return GameData.IsTestingOffline ? null : new BitmapImage(new Uri(this.Player.ImageURL, UriKind.Absolute)); } }
	}
}
