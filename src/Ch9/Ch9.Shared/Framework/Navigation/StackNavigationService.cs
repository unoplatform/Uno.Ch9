using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

namespace Ch9
{
	/// <summary>
	/// This is an implementation of <see cref="IStackNavigationService"/> that uses
	/// an inner <see cref="NavigationService"/> to do most operations.
	/// </summary>
	public sealed class StackNavigationService : IStackNavigationService
	{
		private readonly NavigationService _innerNavigationService;

		public StackNavigationService(NavigationService innerNavigationService)
		{
			_innerNavigationService = innerNavigationService ?? throw new ArgumentNullException(nameof(innerNavigationService));
		}

		public event OnNavigatedEventHandler OnNavigated;

		public string CurrentPageKey => _innerNavigationService.CurrentPageKey;

		public bool CanGoBack => _innerNavigationService.CanGoBack;

		public void GoBack()
		{
			_innerNavigationService.GoBack();

			OnNavigated?.Invoke(this, new OnNavigatedEventArgs(CurrentPageKey));
		}

		public void NavigateTo(string pageKey)
		{
			_innerNavigationService.NavigateTo(pageKey);

			OnNavigated?.Invoke(this, new OnNavigatedEventArgs(pageKey));
		}

		public void NavigateTo(string pageKey, object parameter)
		{
			_innerNavigationService.NavigateTo(pageKey, parameter);

			OnNavigated?.Invoke(this, new OnNavigatedEventArgs(pageKey));
		}

		public IList<PageStackEntry> BackStack => _innerNavigationService.CurrentFrame.BackStack;
	}
}
