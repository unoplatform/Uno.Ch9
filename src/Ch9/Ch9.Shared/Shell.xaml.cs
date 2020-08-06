using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Ch9.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Ch9
{
	public sealed partial class Shell : UserControl
	{
		public static Shell Instance { get; private set; }

		public Shell()
		{
			this.InitializeComponent();

			Instance = this;

			InitializeSafeArea();

			this.Loaded += OnLoaded;
		}

		public Frame Frame => this.RootFrame;

		public NavigationView NavigationView => this.RootNavigationView;

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			NavigationView.ItemInvoked += OnNavigationViewItemInvoked;
			NavigationView.BackRequested += OnNavigationViewBackRequested;

			NavigationView.SelectedItem = NavigationView.MenuItems.First();
		}

		private void OnNavigationViewBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
		{
			var navigationService = App.ServiceProvider.GetInstance<IStackNavigationService>();

			if (navigationService.CanGoBack)
			{
				navigationService.GoBack();

				return;
			}
		}

		private void OnNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
			if (args.InvokedItemContainer is NavigationViewItem item)
			{
				switch (item.Content.ToString())
				{
					case "Recent":
						App.ServiceProvider.GetInstance<IStackNavigationService>().NavigateToAndClearStack(nameof(RecentEpisodesPage));
						break;

					case "Shows":
						App.ServiceProvider.GetInstance<IStackNavigationService>().NavigateToAndClearStack(nameof(ShowsPage));
						break;

					case "About":
						App.ServiceProvider.GetInstance<IStackNavigationService>().NavigateToAndClearStack(nameof(AboutPage));
						break;
				}
			}
		}

		/// <summary>
		/// This method handles the top padding for phones like iPhone X.
		/// </summary>
		private void InitializeSafeArea()
		{
			var full = Windows.UI.Xaml.Window.Current.Bounds;
			var bounds = ApplicationView.GetForCurrentView().VisibleBounds;

			var topPadding = Math.Abs(full.Top - bounds.Top);

			if (topPadding > 0)
			{
				TopPaddingRow.Height = new GridLength(topPadding);
			}
		}
	}
}
