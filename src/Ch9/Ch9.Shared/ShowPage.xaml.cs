using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Ch9.Domain;
using Ch9.ViewModels;
using Uno.Extensions.Specialized;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ch9
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ShowPage : Page
	{
		public ShowPage()
		{
			this.InitializeComponent();

			this.NavigationCacheMode = NavigationCacheMode.Required;

			PostList.RegisterPropertyChangedCallback(ItemsControl.ItemsSourceProperty, OnItemsSourceChanged);
		}

		public ShowPageViewModel ViewModel
		{
			get => DataContext as ShowPageViewModel;
			set => DataContext = value;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			ViewModel = new ShowPageViewModel(e.Parameter as SourceFeed);

			if (PostList.Items.Count > 0 &&
				Windows.UI.Xaml.Window.Current.Bounds.Width >= 800)
			{
				PostList.SelectedIndex = 0;
			}
		}

		private void OnItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
		{
			// Auto select the first element if the items change and the window is wide.
			var listView = sender as ListView;
			var items = listView?.ItemsSource as ICollection;

			if (items?.Count > 0 &&
				Windows.UI.Xaml.Window.Current.Bounds.Width >= 800)
			{
				listView.SelectedIndex = 0;
			}
		}

		private void PostListSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Auto select the first element if the items change and the window is wide.
			var listView = sender as ListView;
			var items = listView?.ItemsSource as ICollection;

			if (Windows.UI.Xaml.Window.Current.Bounds.Width <= 800)
			{
				ScrollViewer scroller = (ScrollViewer)NarrowScrollViewer;

				scroller.ChangeView(
					horizontalOffset: 0,
					verticalOffset: 0,
					zoomFactor: 1,
					disableAnimation: true
				);
			}
		}
	}
}
