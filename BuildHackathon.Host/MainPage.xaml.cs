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
					GameData.Game = await hubProxy.Invoke<Game>("CreateGame");
					
//GameData.Game = new Game();
				}
			}
			catch (HttpRequestException)
			{
				btnStartGame.Content = "Could not connect :( Try again!";
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
    }
}
