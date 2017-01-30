using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Mehspot.Core.Contracts.Wrappers;

namespace Mehspot.Android.Wrappers
{
    public class ActivityHelper: IViewHelper
    {
        Context context;
        ProgressDialog dialog;

        public ActivityHelper (Context context)
        {
            this.context = context;
        }

        public void ShowOverlay (string text)
        {
            dialog = new ProgressDialog (context);
            dialog.SetMessage (text);
            dialog.SetProgressStyle (ProgressDialogStyle.Spinner);
            dialog.Show ();
        }

        public void HideOverlay ()
        {
            if (dialog != null && dialog.IsShowing) {
                dialog.Hide ();
                dialog.Dispose ();
                dialog = null;
            }
        }

        public void ShowAlert (string title, string text)
        {
            Toast.MakeText (context, text, ToastLength.Long).Show ();
        }
    }
}
