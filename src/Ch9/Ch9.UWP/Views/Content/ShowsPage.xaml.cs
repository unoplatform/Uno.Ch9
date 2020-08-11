using System;
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
	public sealed partial class ShowsPage : Page
	{
		public ShowsPage()
		{
			this.InitializeComponent();

			DataContext = new ShowsPageViewModel();
		}

		public ShowsPageViewModel ViewModel => DataContext as ShowsPageViewModel;
	}
}
