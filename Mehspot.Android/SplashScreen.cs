
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using mehspot.Android.Core;
using Mehspot.Core;

namespace Mehspot.Android
{
    [Activity (MainLauncher = true, NoHistory = true)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate (Bundle savedInstanceState)
        {
            MehspotAppContext.Instance.Initialize (new ApplicationDataStorage ());

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.SplashScreen);
            Task startupWork = new Task (() => {
                Task.Delay (1000);  // Simulate a bit of startup work.
            });

            startupWork.ContinueWith (t => {

                Type targetActivityType;
                if (!MehspotAppContext.Instance.AuthManager.IsAuthenticated ()) {
                    targetActivityType = typeof (SignInActivity);
                } else {
                    targetActivityType = typeof (MessageBoardActivity);
                }
                base.StartActivity (new Intent (Application.Context, targetActivityType));

            }, TaskScheduler.FromCurrentSynchronizationContext ());

            startupWork.Start ();

            base.OnCreate (savedInstanceState);
        }
    }
}
