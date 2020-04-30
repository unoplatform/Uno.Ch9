using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Views;
using Xamarin.Essentials;
using Uno.UI;

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
		}
	}
}

