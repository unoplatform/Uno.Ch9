using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Views;
using Windows.UI.Xaml.Navigation;

namespace Ch9
{
	/// <summary>
	/// This is an extended version of <see cref="INavigationService"/> that
	/// notifies when a navigation has been completed.
	/// </summary>
	public interface IStackNavigationService : INavigationService
	{
		/// <summary>
		/// Occurs when a navigation operation has been completed.
		/// </summary>
		event OnNavigatedEventHandler OnNavigated;

		/// <summary>
		/// Gets whether or not a back navigation can be completed.
		/// </summary>
		bool CanGoBack { get; }

		/// <summary>
		/// Gets the navigation back stack.
		/// </summary>
		IList<PageStackEntry> BackStack { get; }

		void NavigateToAndClearStack(string pageKey, object parameter = null);
	}

	public delegate void OnNavigatedEventHandler(IStackNavigationService sender, OnNavigatedEventArgs args);

	public class OnNavigatedEventArgs : EventArgs
	{
		public OnNavigatedEventArgs(string pageKey)
		{
			PageKey = pageKey;
		}

		public string PageKey { get; }
	}
}
