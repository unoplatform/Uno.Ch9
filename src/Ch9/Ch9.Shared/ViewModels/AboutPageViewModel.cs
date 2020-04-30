using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;

namespace Ch9
{
	[Windows.UI.Xaml.Data.Bindable]
	public class AboutPageViewModel : ViewModelBase
	{
		public AboutPageViewModel()
		{
			NavigateToGitHubRepo = new RelayCommand(async () => await Launcher.LaunchUriAsync(new Uri("https://github.com/unoplatform/Uno.Ch9")));
			NavigateToUnoRepo = new RelayCommand(async () => await Launcher.LaunchUriAsync(new Uri("https://github.com/unoplatform")));
		}

		public ICommand NavigateToGitHubRepo { get; }

		public ICommand NavigateToUnoRepo { get; }
	}
}
