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
using Windows.UI.Core;

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

			DataContext = new ShowPageViewModel();

			EpisodeList.RegisterPropertyChangedCallback(ItemsControl.ItemsSourceProperty, OnItemsSourceChanged);
		}

		public ShowPageViewModel ViewModel => DataContext as ShowPageViewModel;

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			ViewModel.OnNavigatedTo(e.Parameter as SourceFeed);
		}

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
			// Set selected episode to null to stop video playing when navigating away
            if (DataContext is ShowPageViewModel vm) vm.Show.SelectedEpisode = null;
        }

        private void OnItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
		{
			// Auto select the first element if the items change and the window is wide.
			var listView = sender as ListView;
			var items = listView?.ItemsSource as ICollection;

			if (items?.Count > 0 &&
				Windows.UI.Xaml.Window.Current.Bounds.Width >= 800)
			{
				// Dispatch item selection on the next loop.
				_ = Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
				{
					listView.SelectedIndex = 0;
				});
			}
		}

        private void VisualStateGroupCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            var vm = DataContext as ShowPageViewModel;

			if (e.NewState?.Name == "NarrowAndSelected")
            {
                if (vm != null) vm.IsNarrowAndSelected = true;
            }
            else
            {
				if (vm != null) vm.IsNarrowAndSelected = false;
			}
        }
	}
}
