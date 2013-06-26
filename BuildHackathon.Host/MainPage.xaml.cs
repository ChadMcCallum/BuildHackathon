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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BuildHackathon.Host
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

		private async void btnStartGame_Click(object sender, RoutedEventArgs e)
		{
			// Connect to our Azure host.
			var hubConnection = new HubConnection("http://buildhackathon.cloudapp.net/signalr");
			var hubProxy = hubConnection.CreateHubProxy("GameHub");
			await hubConnection.Start();
			
			// Get the game object back from Azure.
			var game = await hubProxy.Invoke<dynamic>("CreateGame");
			txtOutput.Text = "Connected! " + game.Id;
			

		}
    }
}
