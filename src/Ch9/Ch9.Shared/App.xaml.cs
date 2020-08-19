﻿using GalaSoft.MvvmLight.Ioc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.UI;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Ch9.ViewModels;
using Xamarin.Essentials;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Uno.Extensions;
using Uno.Logging;

namespace Ch9
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : Application
	{
		public static App Instance { get; private set; }

		private readonly Startup _startup;
		private Frame _rootFrame;

		public static SimpleIoc ServiceProvider { get; } = SimpleIoc.Default;

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			Instance = this;

			// Uncomment this if you want to set a default theme.
			// this.RequestedTheme = ApplicationTheme.Dark;

			_startup = new Startup();

#if !DEBUG && WINDOWS_UWP
			AppCenter.Start("68d4e1c1-d72c-491e-9c16-5302d9521fb1", typeof(Analytics), typeof(Crashes));
#endif

			ConfigureFilters(global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);

			this.InitializeComponent();
			this.Suspending += OnSuspending;
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// this.DebugSettings.EnableFrameRateCounter = true;
			}
#endif
			_rootFrame = Windows.UI.Xaml.Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (_rootFrame == null)
			{
#if !DEBUG && __IOS__
				AppCenter.Start("c1c95ee1-7532-486b-a542-cab21f444edb", typeof(Analytics), typeof(Crashes));
#endif

				_startup.Initialize(ServiceProvider);

				ConfigureViewSize();
				ConfigureStatusBar();

				// Create a Frame to act as the navigation context and navigate to the first page
				_rootFrame = new Frame();

				_rootFrame.NavigationFailed += OnNavigationFailed;

				// Place the frame in the current Window
				Windows.UI.Xaml.Window.Current.Content = _rootFrame;
			}

			if (e.PrelaunchActivated == false)
			{
				if (_rootFrame.Content == null)
				{
					// When the navigation stack isn't restored navigate to the first page,
					// configuring the new page by passing required information as a navigation
					// parameter
					ConfigureSystemBackVisibility();
					ConfigureBackRequests();
					ConfigureOrientation();
					ConfigureKeyHookUwp();

					ServiceProvider.GetInstance<IStackNavigationService>().NavigateTo(nameof(MainPage));
				}

				// Ensure the current window is active
				Windows.UI.Xaml.Window.Current.Activate();
			}
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();

			deferral.Complete();
		}

		private void ConfigureViewSize()
		{
#if WINDOWS_UWP
            ApplicationView.PreferredLaunchViewSize = new Size(1024, 768);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 480));
#endif
		}

		private void ConfigureKeyHookUwp()
		{
#if WINDOWS_UWP
            Window.Current.CoreWindow.CharacterReceived += CoreWindowCharacterReceived;

            void CoreWindowCharacterReceived(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.CharacterReceivedEventArgs args)
            {
                if (args.KeyCode == 27) //Escape
                {
                    if ((_rootFrame.Content as FrameworkElement)?.DataContext is ShowPageViewModel showPage &&
                        showPage.Show.IsVideoFullWindow)
                    {
                        showPage.Show.IsVideoFullWindow = false;
                    }
                    else if ((_rootFrame.Content as FrameworkElement)?.DataContext is MainPageViewModel mainPage &&
                             mainPage.Show.IsVideoFullWindow)
                    {
                        mainPage.Show.IsVideoFullWindow = false;
                    }
                }
            }
#endif
		}

		private void ConfigureStatusBar()
		{
			var resources = Windows.UI.Xaml.Application.Current.Resources;

#if WINDOWS_UWP
            var hasStatusBar = false;
#else
			var hasStatusBar = true;

			StatusBar.GetForCurrentView().ForegroundColor = this.RequestedTheme == ApplicationTheme.Dark
				? Windows.UI.Colors.White
				: Windows.UI.Colors.Black;
#endif

			var statusBarHeight = hasStatusBar ? Windows.UI.ViewManagement.StatusBar.GetForCurrentView().OccludedRect.Height : 0;

			resources.Add("StatusBarDouble", (double)statusBarHeight);
			resources.Add("StatusBarThickness", new Thickness(0, statusBarHeight, 0, 0));
			resources.Add("StatusBarGridLength", new GridLength(statusBarHeight, GridUnitType.Pixel));
		}

		private void ConfigureOrientation()
		{
			if (DeviceInfo.Idiom == DeviceIdiom.Phone)
			{
				DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
			}

			var simpleOrientationSensor = SimpleOrientationSensor.GetDefault();

			if (simpleOrientationSensor != null)
			{
				simpleOrientationSensor.OrientationChanged += OrientationChanged;
			}

			void OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
			{
				try
				{
					var isLandscape = args.Orientation.IsOneOf(
						SimpleOrientation.Rotated270DegreesCounterclockwise,
						SimpleOrientation.Rotated90DegreesCounterclockwise
					);

					if ((_rootFrame.Content as FrameworkElement)?.DataContext is ShowPageViewModel showPage &&
						showPage.Show.SelectedEpisode != null)
					{
						ToVideoFullWindow(showPage.Show, isLandscape);
					}
					else if ((_rootFrame.Content as FrameworkElement)?.DataContext is MainPageViewModel mainPage &&
							 mainPage.Show.SelectedEpisode != null)
					{
						ToVideoFullWindow(mainPage.Show, isLandscape);
					}
				}
				catch (Exception ex)
				{
					this.Log().ErrorIfEnabled(() => $"Error in OrientationChanged subscription: {ex.ToString()}");
				}
			}
		}

		/// <summary>
		/// Sets the video to fullWindow depending on screen orientation
		/// </summary>
		/// <param name="showVm">The show vm for which we will change its property IsVideoFullWindow</param>
		/// <param name="isLandscape">Determine if the device is oriented in landscape</param>
		private void ToVideoFullWindow(ShowViewModel showVm, bool isLandscape)
		{
			if (showVm == null)
				return;

			//Set display orientation to none, thus screen can handle LandscapeFlipped
			DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
			showVm.IsVideoFullWindow = isLandscape;
		}

		/// <summary>
		/// Sets the visibility of the system UI's back button based on the navigation service.
		/// </summary>
		private void ConfigureSystemBackVisibility()
		{
			var navigationService = ServiceProvider.GetInstance<IStackNavigationService>();

			void OnNavigated(IStackNavigationService sender, OnNavigatedEventArgs args)
			{
				SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = sender.CanGoBack
					? AppViewBackButtonVisibility.Visible
					: AppViewBackButtonVisibility.Collapsed;
			}

			navigationService.OnNavigated += OnNavigated;
		}

		/// <summary>
		/// Hooks the system back button with the navigation service.
		/// </summary>
		private void ConfigureBackRequests()
		{
			void OnBackRequested(object sender, BackRequestedEventArgs e)
			{
				// ShowPage hook back request
				if ((_rootFrame.Content as FrameworkElement)?.DataContext is ShowPageViewModel showPage &&
					showPage.Show.SelectedEpisode != null && showPage.IsNarrowAndSelected)
				{
					e.Handled = true;

					//Dismiss episode only if we are not in full screen
					if (!showPage.Show.IsVideoFullWindow)
					{
						showPage.Show.DismissSelectedEpisode.Execute(null);
						//don't navigate back as NarrowAndSelected
						return;
					}

					showPage.Show.IsVideoFullWindow = false;
					return;
				}

				var navigationService = ServiceProvider.GetInstance<IStackNavigationService>();

				if (navigationService.CanGoBack)
				{
					e.Handled = true;

					navigationService.GoBack();

					return;
				}

				// MainPage hook back request
				if ((_rootFrame.Content as FrameworkElement)?.DataContext is MainPageViewModel mainPage &&
					mainPage.Show.SelectedEpisode != null)
				{
					if (!mainPage.Show.IsVideoFullWindow)
					{
						mainPage.Show.DismissSelectedEpisode.Execute(null);
					}
					else
					{
						mainPage.Show.IsVideoFullWindow = false;
					}

					e.Handled = true;
				}
			}

			SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
		}

		private static bool _isActivityBackgroundCleared;
		private static DisplayOrientations _previousOrientation;
		public static void OnFullscreenChanged(bool isFullscreen)
		{
#if __ANDROID__
			// This will reset the window background from the splashscreen to a black background.
			if (isFullscreen && !_isActivityBackgroundCleared)
			{
				(ContextHelper.Current as Android.App.Activity).Window.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.Black));
				_isActivityBackgroundCleared = true;
			}
#endif

			if (DeviceInfo.Idiom == DeviceIdiom.Phone)
			{
				if (isFullscreen)
				{
					if (DisplayInformation.AutoRotationPreferences == DisplayOrientations.None) return;

					DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
				}
				else
				{
					DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
				}
			}
		}

		/// <summary>
		/// Configures global logging
		/// </summary>
		/// <param name="factory"></param>
		static void ConfigureFilters(ILoggerFactory factory)
		{
			factory
				.WithFilter(new FilterLoggerSettings
					{
						{ "Uno", Microsoft.Extensions.Logging.LogLevel.Warning },
						{ "Windows", Microsoft.Extensions.Logging.LogLevel.Warning },

						// Debug JS interop
						// { "Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug },

						// Generic Xaml events
						// { "Windows.UI.Xaml", LogLevel.Debug },
						// { "Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug },
						// { "Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.UIElement", LogLevel.Debug },

						// Layouter specific messages
						// { "Windows.UI.Xaml.Controls", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Panel", LogLevel.Debug },
						// { "Windows.Storage", LogLevel.Debug },

						// Binding related messages
						// { "Windows.UI.Xaml.Data", LogLevel.Debug },

						// DependencyObject memory references tracking
						// { "ReferenceHolder", LogLevel.Debug },

						// ListView-related messages
						// { "Windows.UI.Xaml.Controls.ListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.GridView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelLayout", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.NativeListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListViewBaseSource", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.ListViewBaseInternalContainer", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.NativeListViewBaseAdapter", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.BufferViewCache", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelGenerator", LogLevel.Debug }, //WASM
					}
				)
#if DEBUG
				.AddConsole(Microsoft.Extensions.Logging.LogLevel.Debug);
#else
				.AddConsole(Microsoft.Extensions.Logging.LogLevel.Information);
#endif
		}
	}
}
