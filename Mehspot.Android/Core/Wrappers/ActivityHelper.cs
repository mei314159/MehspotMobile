using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Mehspot.Core.Contracts.Wrappers;

namespace Mehspot.AndroidApp.Wrappers
{
	public class ActivityHelper : IViewHelper
	{
		Context context;
		ProgressDialog dialog;

		public ActivityHelper(Context context)
		{
			this.context = context;
		}

		public void ShowOverlay(string text)
		{
			dialog = new ProgressDialog(context);
			dialog.SetMessage(text);
			dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
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

		public void ShowAlert(string title, string text)
		{
			Toast.MakeText(context, text, ToastLength.Long).Show();
		}

		public void ShowPrompt(string title, string text, string positiveButtonTitle, Action positiveAction)
		{
			var alert = new AlertDialog.Builder(context);
			alert.SetTitle(title);
			alert.SetMessage(text);
			alert.SetNegativeButton("Cancel", (sender, e) => { });
			alert.SetPositiveButton(positiveButtonTitle, (senderAlert, args) => positiveAction());
			alert.Create().Show();
		}
	}
}
