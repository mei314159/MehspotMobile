
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Mehspot.AndroidApp.Core;
using Mehspot.Core;

namespace Mehspot.AndroidApp
{
	[Activity(MainLauncher = true, NoHistory = true, Theme = "@android:style/Theme.Holo.NoActionBar")]
	public class SplashScreenActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			MehspotAppContext.Instance.Initialize(new ApplicationDataStorage());

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.SplashScreen);
			Task startupWork = new Task(() =>
			{
				Task.Delay(1000);  // Simulate a bit of startup work.
			});

			startupWork.ContinueWith(t =>
			{

				Type targetActivityType;
				if (!MehspotAppContext.Instance.AuthManager.IsAuthenticated())
				{
					targetActivityType = typeof(SignInActivity);
				}
				else
				{
					targetActivityType = typeof(MainActivity);
				}
				base.StartActivity(new Intent(Application.Context, targetActivityType));

			}, TaskScheduler.FromCurrentSynchronizationContext());

			startupWork.Start();

			base.OnCreate(savedInstanceState);
		}
	}
}
