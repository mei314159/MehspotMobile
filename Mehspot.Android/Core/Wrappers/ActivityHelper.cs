using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Mehspot.Core.Contracts.Wrappers;

namespace Mehspot.AndroidApp.Wrappers
{
	public class ActivityHelper : IViewHelper
	{
		Activity activity;
		ProgressDialog dialog;

		public ActivityHelper(Activity activity)
		{
			this.activity = activity;
		}

		public void ShowOverlay(string text, bool opaque = false)
		{
			activity.RunOnUiThread(() =>
				{
					dialog = new ProgressDialog(activity);
					dialog.SetMessage(text);
					dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
					dialog.SetCancelable(false);
					dialog.Show();
				});
		}

		public void HideOverlay()
		{
			if (dialog != null && dialog.IsShowing)
			{
				activity.RunOnUiThread(() =>
				{
					dialog.Hide();
					dialog.Dispose();
					dialog = null;
				});
			}
		}

		public void ShowAlert(string title, string text, Action action = null)
		{
			activity.RunOnUiThread(() =>
				{
					var alert = new AlertDialog.Builder(activity);
					alert.SetTitle(title);
					alert.SetMessage(text);
					alert.SetPositiveButton("OK", (senderAlert, args) => action());
					alert.Create().Show();
				});
		}

		public void ShowPrompt(string title, string text, string positiveButtonTitle, Action positiveAction)
		{
			activity.RunOnUiThread(() =>
				{
					var alert = new AlertDialog.Builder(activity);
					alert.SetTitle(title);
					alert.SetMessage(text);
					alert.SetNegativeButton("Cancel", (sender, e) => { });
					alert.SetPositiveButton(positiveButtonTitle, (senderAlert, args) => positiveAction());
					alert.Create().Show();
				});
		}
	}
}
