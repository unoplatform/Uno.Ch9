using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Ch9
{
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();

			this.NavigationCacheMode = NavigationCacheMode.Required;

			DataContext = new MainPageViewModel();
		}

		public MainPageViewModel ViewModel => DataContext as MainPageViewModel;

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			ViewModel.OnNavigatedTo();
		}

		private void OnPostItemClick(object sender, ItemClickEventArgs e)
		{
			ViewModel.ShowPost.Execute(e.ClickedItem);
		}
	}
}
