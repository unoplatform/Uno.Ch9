using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Ch9.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Ch9.Views
{
	public sealed partial class RecentEpisodesPage : Page
	{
		public RecentEpisodesPage()
		{
			this.InitializeComponent();

			DataContext = new RecentEpisodesPageViewModel();

			//EpisodeList.RegisterPropertyChangedCallback(ItemsControl.ItemsSourceProperty, OnItemsSourceChanged);
		}

		public RecentEpisodesPageViewModel ViewModel => DataContext as RecentEpisodesPageViewModel;

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			ViewModel.OnNavigatedTo();
		}

		private void OnItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
		{
			// Auto select the first element if the items change and the window is wide.
			var listView = sender as ListView;
			var items = listView?.ItemsSource as ICollection;

			if (items?.Count > 0 && Windows.UI.Xaml.Window.Current.Bounds.Width >= 800)
			{
				listView.SelectedIndex = 0;
			}
		}
	}
}
