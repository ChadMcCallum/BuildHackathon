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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace BuildHackathon.Host
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class CurrentGamePage : BuildHackathon.Host.Common.LayoutAwarePage
	{
		public CurrentGamePage()
		{
			this.InitializeComponent();
			this.DataContext = this;
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
			if (GameData.Game == null)
			{
				GameData.Game = pageState["Game"] as Game;
			}

			// Allow people to join the game.
			imgQrCode.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(GameData.Game.QRCodeImage, UriKind.Absolute));
			txtQrCodeLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;
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
		}
	}
}
