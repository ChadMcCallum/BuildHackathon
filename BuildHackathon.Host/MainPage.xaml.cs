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
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using BuildHackathon.Shared;
using System.Net.Http;
using System.ComponentModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BuildHackathon.Host
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		private void SendPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		public bool GameIsNull { get { return GameData.Game == null; } }

		private void UpdateUI()
		{
			if (GameIsNull)
			{
				btnStartGame.Content = "Start Game";
			}
			else
			{
				btnStartGame.Content = "Back To Current Game";
			}

			SendPropertyChanged("GameIsNull");
		}

        public MainPage()
        {
            this.InitializeComponent();
			this.DataContext = this;

			if (GameData.CustomTwitterHandles != null)
			{
				txtCustom1.Text = GameData.CustomTwitterHandles.Count > 0 ? GameData.CustomTwitterHandles[0] : string.Empty;
				txtCustom2.Text = GameData.CustomTwitterHandles.Count > 1 ? GameData.CustomTwitterHandles[1] : string.Empty;
				txtCustom3.Text = GameData.CustomTwitterHandles.Count > 2 ? GameData.CustomTwitterHandles[2] : string.Empty;
				txtCustom4.Text = GameData.CustomTwitterHandles.Count > 3 ? GameData.CustomTwitterHandles[3] : string.Empty;
				txtCustom5.Text = GameData.CustomTwitterHandles.Count > 4 ? GameData.CustomTwitterHandles[4] : string.Empty;
				txtCustom6.Text = GameData.CustomTwitterHandles.Count > 5 ? GameData.CustomTwitterHandles[5] : string.Empty;
			}

			if (GameData.GameType != null)
			{
				switch (GameData.GameType.Value)
				{
					default:
					case GameType.PlayersOnly: radioUsersOnly.IsChecked = true; break;
					case GameType.CelebsOnly: radioCustomOnly.IsChecked = true; break;
					case GameType.PlayersAndCelebs: radioUsersAndCustom.IsChecked = true; break;
				}
			}
			else
			{
				radioUsersAndCustom.IsChecked = true;
			}
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
			UpdateUI();
        }

		private async void btnStartGame_Tapped(object sender, RoutedEventArgs e)
		{
			var customAccounts = new List<string>();
			GameType gameType;
			if (!radioUsersOnly.IsChecked.Value)
			{
				AddValidTwitterHandleToList(ref customAccounts, txtCustom1.Text);
				AddValidTwitterHandleToList(ref customAccounts, txtCustom2.Text);
				AddValidTwitterHandleToList(ref customAccounts, txtCustom3.Text);
				AddValidTwitterHandleToList(ref customAccounts, txtCustom4.Text);
				AddValidTwitterHandleToList(ref customAccounts, txtCustom5.Text);
				AddValidTwitterHandleToList(ref customAccounts, txtCustom6.Text);

				if (customAccounts.Count < 2)
				{
					btnStartGame.Content = "2 Accounts Required";
					return;
				}

				if (radioCustomOnly.IsChecked.Value)
					gameType = GameType.CelebsOnly;
				else
					gameType = GameType.PlayersAndCelebs;
			}
			else
			{
				gameType = GameType.PlayersOnly;
			}

			GameData.CustomTwitterHandles = customAccounts;
			GameData.GameType = gameType;

			// Don't allow them to click again while we process.
			btnStartGame.IsEnabled = false;

			try
			{
				// If we have not started a game yet.
				if (GameIsNull)
				{
					btnStartGame.Content = "Connecting...";

					// Connect to our Azure host.
					var hubConnection = new HubConnection("http://buildhackathon.cloudapp.net/signalr");
					var hubProxy = hubConnection.CreateHubProxy("GameHub");
					await hubConnection.Start();
					
					// Save the proxy.
					GameData.HubProxy = hubProxy;

					// Get the game object back from Azure.
					GameData.Game = await hubProxy.Invoke<Game>("CreateGame", gameType, customAccounts.ToArray());
					
//GameData.Game = new Game();
				}
			}
			catch (HttpRequestException)
			{
				btnStartGame.Content = "Could not connect :(";
				return;
			}
			finally
			{
				// Allow them to click button again now that we are done processing.
				btnStartGame.IsEnabled = true;

				// Update button's text.
				SendPropertyChanged("GameIsNull");
			}

			NavigateToGamePage();
		}

		private void NavigateToGamePage()
		{
			// Navigate to the game page.
			if (this.Frame.CurrentSourcePageType != typeof(CurrentGamePage))
				this.Frame.Navigate(typeof(CurrentGamePage));
		}

		private void btnEndGame_Tapped(object sender, TappedRoutedEventArgs e)
		{
			// Send a message to any remaining clients to cancel the game.
			GameData.HubProxy.Invoke("CancelGame");

			GameData.Game = null;
			GameData.HubProxy = null;
			UpdateUI();
		}

		private void AddValidTwitterHandleToList(ref List<string> list, string handle)
		{
			handle = handle.Trim();
			if (!string.IsNullOrWhiteSpace(handle) && !list.Contains(handle))
				list.Add(handle);
		}
    }
}
