using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BuildHackathon.Shared;
using System.ComponentModel;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Windows.UI.Core;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace BuildHackathon.Host
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class CurrentGamePage : BuildHackathon.Host.Common.LayoutAwarePage, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		public void SendPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		public CurrentGamePage()
		{
			this.InitializeComponent();
			this.DataContext = this;
		}

		public List<ClientPlayer> BlueTeamPlayers 
		{ 
			get 
			{
				var list = new List<ClientPlayer>();
				foreach (var player in GameData.Game.BlueTeam.Players.OrderByDescending(p => p.Score))
					list.Add(new ClientPlayer(player));
				return list;
			} 
		}
		public List<ClientPlayer> RedTeamPlayers
		{
			get
			{
				var list = new List<ClientPlayer>();
				foreach (var player in GameData.Game.RedTeam.Players.OrderByDescending(p => p.Score))
					list.Add(new ClientPlayer(player));
				return list;
			}
		}

		/// <summary>
		/// Populates the page with content passed during navigation.  Any saved state is also
		/// provided when recreating a page from a prior session.
		/// </summary>
		/// <param name="navigationParameter">The parameter value passed to
		/// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
		/// </param>
		/// <param name="pageState">A dictionary of state preserved by this page during an earlier
		/// session.  This will be null the first time a page is visited.</param>
		protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
		{
			// Rehydrate our objects if needed.
			if (GameData.Game == null) { GameData.Game = pageState["Game"] as Game; }
			if (GameData.HubProxy == null) { GameData.HubProxy = pageState["HubProxy"] as IHubProxy; }

			// Hookup any event handlers.
			GameData.HubProxy.On<Question>("NewQuestion", question =>
				this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					txtTweet.Text = question.Tweet;
				}));

			GameData.HubProxy.On<Team[]>("UpdateScore", teams =>
				this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					UpdateTeams(teams);
				}));

			GameData.HubProxy.On<Team[]>("NewPlayer", teams =>
				this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					UpdateTeams(teams);
				}));

			GameData.HubProxy.On<Team[]>("RemovePlayer", teams =>
				this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					UpdateTeams(teams);
				}));

			GameData.HubProxy.On<string>("EndGame", message =>
				this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					txtTweet.Text = message;
					GameData.Game = null;
					GameData.HubProxy = null;
				}));

			RefreshAllData();
		}

		/// <summary>
		/// Preserves state associated with this page in case the application is suspended or the
		/// page is discarded from the navigation cache.  Values must conform to the serialization
		/// requirements of <see cref="SuspensionManager.SessionState"/>.
		/// </summary>
		/// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
		protected override void SaveState(Dictionary<String, Object> pageState)
		{
			pageState["Game"] = GameData.Game;
			pageState["HubProxy"] = GameData.HubProxy;
		}

		private void UpdateTeams(Team[] teams)
		{
			foreach (var team in teams)
			{
				if (team.Name.Equals("Blue", StringComparison.CurrentCultureIgnoreCase))
				{
					GameData.Game.BlueTeam.Players.Clear();
					GameData.Game.BlueTeam.Players = team.Players;
					txtBlueTeamScore.Text = team.Score.ToString();
				}
				else if (team.Name.Equals("Red", StringComparison.CurrentCultureIgnoreCase))
				{
					GameData.Game.RedTeam.Players.Clear();
					GameData.Game.RedTeam.Players = team.Players;
					txtRedTeamScore.Text = team.Score.ToString();
				}
			}

//InsertTestData();

			SendPropertyChanged("BlueTeamPlayers");
			SendPropertyChanged("RedTeamPlayers");
		}

		private void RefreshAllData()
		{
			// Allow people to join the game.
			imgQrCode.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(GameData.Game.QRCodeImage, UriKind.Absolute));
			txtQrCodeLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;

			UpdateTeams(new Team[] { GameData.Game.BlueTeam, GameData.Game.RedTeam });
		}

		private void InsertTestData()
		{
			var player = new Player("player1") { Name = "Player 1", Score = 25 };
			GameData.Game.BlueTeam.AddPlayer(player);

			player = new Player("player1") { Name = "Batman", Score = 250 };
			GameData.Game.BlueTeam.AddPlayer(player);

			player = new Player("player1") { Name = "Yoda", Score = 2500 };
			GameData.Game.BlueTeam.AddPlayer(player);
		}
	}
}
