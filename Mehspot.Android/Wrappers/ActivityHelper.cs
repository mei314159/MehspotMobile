using System;
using Android.Content;
using Android.Widget;
using Mehspot.Core.Contracts.Wrappers;

namespace Mehspot.Android.Wrappers
{
    public class ActivityHelper: IViewHelper
    {
        Context context;

        public ActivityHelper (Context context)
        {
            this.context = context;
        }

        public void HideOverlay ()
        {
        }

        public void ShowAlert (string title, string text)
        {
            Toast.MakeText (context, text, ToastLength.Long).Show ();
        }

        public void ShowOverlay (string text)
        {
        }
    }
}
