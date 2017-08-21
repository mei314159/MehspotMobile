using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Mehspot.Core.Contracts.Wrappers;

namespace Mehspot.AndroidApp.Wrappers
{
    public class ActivityHelper : IViewHelper
    {
        Handler handler;
        ProgressDialog dialog;
        readonly Context context;

        public ActivityHelper(Context context)
        {
            this.context = context;
            this.handler = new Handler(Android.App.Application.Context.MainLooper);
        }

        public void ShowOverlay(string text, bool opaque = false)
        {
            dialog = new ProgressDialog(context);
            dialog.SetMessage(text);
            dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            dialog.SetCancelable(false);
            dialog.Show();
        }

        public void HideOverlay()
        {
            if (dialog != null && dialog.IsShowing)
            {
                dialog.Hide();
                dialog.Dispose();
                dialog = null;
            }
        }

        public void ShowAlert(string title, string text, Action action = null)
        {
            handler.Post(() =>
                {
                    var alert = new AlertDialog.Builder(context);
                    alert.SetTitle(title);
                    alert.SetMessage(text);
                    alert.SetPositiveButton("OK", (senderAlert, args) => action?.Invoke());
                    alert.Create().Show();
                });
        }

        public void ShowPrompt(string title, string text, string positiveButtonTitle, Action positiveAction)
        {
            handler.Post(() =>
                {
                    var alert = new AlertDialog.Builder(context);
                    alert.SetTitle(title);
                    alert.SetMessage(text);
                    alert.SetNegativeButton("Cancel", (sender, e) => { });
                    alert.SetPositiveButton(positiveButtonTitle, (senderAlert, args) => positiveAction());
                    alert.Create().Show();
                });
        }
    }
}
