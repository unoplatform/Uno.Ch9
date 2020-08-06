using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace Ch9.ViewModels
{
	[Windows.UI.Xaml.Data.Bindable]
	public class ShellViewModel : ViewModelBase
	{
		//public ShellViewModel()
		//{
		//	NavigateToGitHubRepo = new RelayCommand(async () => await Launcher.LaunchUriAsync(new Uri("https://github.com/unoplatform/Uno.Ch9")));
		//	NavigateToUnoRepo = new RelayCommand(async () => await Launcher.LaunchUriAsync(new Uri("https://github.com/unoplatform")));
		//}

		//private bool _isNarrowAndSelected;
		//public bool IsPanelOpen
		//{
		//	get => _isNarrowAndSelected;
		//	set => Set(() => IsNarrowAndSelected, ref _isNarrowAndSelected, value);
		//}
	}
}
