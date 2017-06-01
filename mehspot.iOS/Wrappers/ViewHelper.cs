using System;
using Mehspot.iOS.Views;
using Mehspot.Core.Contracts.Wrappers;
using UIKit;

namespace Mehspot.iOS.Wrappers
{
    public class ViewHelper:IViewHelper
    {
        private LoadingOverlay loadingOverlay;
        readonly UIView view;

        public ViewHelper (UIView view)
        {
            this.view = view;
        }

        public void ShowAlert (string title, string text)
        {
            var avAlert = new UIAlertView (title, text, (IUIAlertViewDelegate)null, "OK", null);
            avAlert.Show ();
        }

        public void ShowOverlay (string text)
        {
            HideOverlay ();
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay (UIScreen.MainScreen.Bounds, text);
            view.Add (loadingOverlay);
            view.BringSubviewToFront (loadingOverlay);
        }

        public void HideOverlay ()
        {
            if (loadingOverlay != null) {
                loadingOverlay.Hide ();
                loadingOverlay.Dispose ();
                loadingOverlay = null;
            }
        }
    }

}
