using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Views;
using Xamarin.Essentials;
using Uno.UI;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Ch9.Droid
{
	[Activity(
		Name = "ch9.droid.MainActivity",
		MainLauncher = true,
		ConfigurationChanges = ActivityHelper.AllConfigChanges,
		WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden,
		ResizeableActivity = false,
		LaunchMode = LaunchMode.SingleTask
	)]
	public class MainActivity : Windows.UI.Xaml.ApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			Platform.Init(this, bundle);

#if !DEBUG
			AppCenter.Start("45a416ab-c847-473c-a370-3ae35ab38ac5", typeof(Analytics), typeof(Crashes));
#endif
		}
	}
}

