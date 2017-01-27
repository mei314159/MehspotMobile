
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using mehspot.Android.Core;

namespace Mehspot.Android
{
    [Activity (MainLauncher = true, NoHistory = true)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.SplashScreen);

            Task startupWork = new Task (() => {
                Task.Delay (2000);  // Simulate a bit of startup work.
            });

            startupWork.ContinueWith (t => {
                var authManager = new mehspot.Core.Auth.AuthenticationManager (new ApplicationDataStorage ());

                Type targetActivityType;
                if (!authManager.IsAuthenticated ()) {
                    targetActivityType = typeof (SignInActivity);
                } else {
                    targetActivityType = typeof (MainActivity);
                }
                base.StartActivity (new Intent (Application.Context, targetActivityType));

            }, TaskScheduler.FromCurrentSynchronizationContext ());

            startupWork.Start ();
        }
    }
}
