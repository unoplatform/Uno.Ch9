using System;
using System.Collections.Generic;
using System.Text;

namespace Ch9
{
	public static class AnalyticsService
	{
		public static void Initialize()
		{
#if __IOS__ || __ANDROID__ || WINDOWS_UWP
#if __IOS__
			var id = "c1c95ee1-7532-486b-a542-cab21f444edb";
#elif __ANDROID__
			var id = "45a416ab-c847-473c-a370-3ae35ab38ac5";
#elif WINDOWS_UWP
			var id = "68d4e1c1-d72c-491e-9c16-5302d9521fb1";
#endif
			Microsoft.AppCenter.AppCenter.Start(
				id,
				typeof(Microsoft.AppCenter.Analytics.Analytics),
				typeof(Microsoft.AppCenter.Crashes.Crashes));
#elif __WASM__
			Uno.Foundation.WebAssemblyRuntime.InvokeJS("Uno.UI.Demo.Analytics.reportPageView('main');");
#endif
		}

		public static void TrackView(string viewName)
		{
			Microsoft.AppCenter.Analytics.Analytics.TrackEvent(viewName);

#if __WASM__
			Uno.Foundation.WebAssemblyRuntime.InvokeJS($"Uno.UI.Demo.Analytics.reportPageView('{viewName}');");
#endif
		}
	}
}
