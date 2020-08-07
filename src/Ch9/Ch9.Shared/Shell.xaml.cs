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

namespace Ch9
{
	public sealed partial class Shell : UserControl
	{
		private readonly Dictionary<string, Frame> _frames = new Dictionary<string, Frame>();

		private readonly Dictionary<string, Type> _tabs = new Dictionary<string, Type>
		{
			{ "Recent", typeof(RecentEpisodesPage) },
			{ "Shows", typeof(ShowsPage) },
			{ "About", typeof(AboutPage) },
		};

		private NavigationViewItem _activeTab;

		public Shell()
		{
			this.InitializeComponent();

			Instance = this;

			InitializeSafeArea();

			this.Loaded += OnLoaded;
		}

		public static Shell Instance { get; private set; }

		public NavigationView NavigationView => this.RootNavigationView;

		public EventHandler<Frame> Navigated;

		public void NavigateTo(string tabKey, Type pageType = null, object parameter = null)
		{
			var item = NavigationView
				.MenuItems
				.Select(s => s as NavigationViewItem)
				.Single(s => s.Tag.ToString() == tabKey);

			NavigateTo(item, pageType, parameter);
		}

		public void NavigateTo(Type pageType, object parameter)
		{
			NavigateTo(_activeTab, pageType, parameter);
		}

		public bool TryGetActiveViewModel<TViewModel>(out TViewModel viewModel)
		{
			var activeFrame = RootContent.Content as Frame;
			var dataContext = (activeFrame?.Content as FrameworkElement)?.DataContext;

			if (dataContext is TViewModel model)
			{
				viewModel = model;

				return true;
			}

			viewModel = default(TViewModel);

			return false;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			InitializeTabs();
		}

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

		private void InitializeTabs()
		{
			if (NavigationView.MenuItems.Count == 0)
			{
				foreach (var tab in _tabs)
				{
					NavigationView.MenuItems.Add(new NavigationViewItem() { Content = tab.Key, Tag = tab.Key });
				}


				NavigationView.ItemInvoked += OnNavigationViewItemInvoked;
				NavigationView.BackRequested += OnNavigationViewBackRequested;

				var initialSelection = (NavigationViewItem)NavigationView.MenuItems.First();
				NavigateTo(initialSelection);
			}
		}

		private void NavigateTo(NavigationViewItem item, Type pageType = null, object parameter = null)
		{
			var tabKey = item.Tag as string;

			if (tabKey == null)
			{
				return;
			}

			if (item == _activeTab && pageType == null)
			{
				return;
			}

			if (_tabs.TryGetValue(tabKey, out var rootPageType))
			{
				var targetPageType = pageType ?? rootPageType;

				if (!_frames.TryGetValue(tabKey, out var frame))
				{
					frame = new Frame();
					_frames.Add(tabKey, frame);
				}

				if (frame.Content == null)
				{
					frame.Navigate(targetPageType, parameter);
				}
				else if (frame.Content.GetType() != targetPageType)
				{
					frame.Navigate(targetPageType, parameter);

					if (targetPageType == rootPageType)
					{
						// Go back to the root page.
						var backStack = frame.BackStackDepth;

						for (var i = 0; i < backStack; i++)
						{
							frame.GoBack();
						}
					}
				}

				this.RootContent.Content = frame;
				NavigationView.SelectedItem = item;

				UpdateBackButtonVisibility();

				_activeTab = item;

				Navigated?.Invoke(this, frame);
			}
		}

		private void OnNavigationViewBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
		{
			var activeFrame = this.RootContent.Content as Frame;

			if (activeFrame?.CanGoBack ?? false)
			{
				activeFrame.GoBack();

				UpdateBackButtonVisibility();

				Navigated?.Invoke(this, activeFrame);
			}
		}

		private void OnNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
			if (args.InvokedItemContainer is NavigationViewItem item)
			{
				NavigateTo(item);
			}
		}

		private void UpdateBackButtonVisibility()
		{
			var activeFrame = this.RootContent.Content as Frame;

			if (activeFrame != null)
			{
				NavigationView.IsBackButtonVisible = activeFrame.CanGoBack
						   ? NavigationViewBackButtonVisible.Visible
						   : NavigationViewBackButtonVisible.Collapsed;
			}
		}
	}
}
