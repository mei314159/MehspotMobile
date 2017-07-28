
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
			base.OnCreate(savedInstanceState);
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.SplashScreen);

		}

		protected override void OnStart()
		{
			base.OnStart();
            RunAsync();
		}

		async Task RunAsync()
		{
			Task.Delay(100).ContinueWith(async (arg) =>
			{
				MehspotAppContext.Instance.Initialize(new ApplicationDataStorage());
				Type targetActivityType;
				if (!MehspotAppContext.Instance.AuthManager.IsAuthenticated())
				{
					targetActivityType = typeof(SignInActivity);
				}
				else
				{
					targetActivityType = typeof(MainActivity);
				}

				RunOnUiThread(() => this.StartActivity(new Intent(Application.Context, targetActivityType)));

			});

		}
	}
}
