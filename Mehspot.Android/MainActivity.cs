using Android.App;
using Android.Widget;
using Android.OS;

namespace Mehspot.Android
{
    [Activity (Label = "Mehspot", MainLauncher = true, Icon = "@mipmap/ic_launcher")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button> (Resource.Id.myButton);

            button.Click += delegate { button.Text = string.Format ("{0} clicks!", count++); };
        }
    }
}

