using System;
using Mehspot.iOS.Views;
using Mehspot.Core.Contracts.Wrappers;
using UIKit;
using CoreGraphics;

namespace Mehspot.iOS.Wrappers
{
	public class ViewHelper : IViewHelper
	{
		private LoadingOverlay loadingOverlay;
		readonly UIView view;

		public ViewHelper(UIView view)
		{
			this.view = view;
		}

		public void ShowAlert(string title, string text)
		{
			view.InvokeOnMainThread(() =>
			{
				var avAlert = new UIAlertView(title, text, (IUIAlertViewDelegate)null, "OK", null);
				avAlert.Show();
			});
		}

		public void ShowPrompt(string title, string text, string positiveButtonTitle, Action positiveAction)
		{
			view.InvokeOnMainThread(() =>
			   {
				   var avAlert = new UIAlertView(title, text, (IUIAlertViewDelegate)null, "Cancel", new[] { positiveButtonTitle });
				   avAlert.Clicked += (sender, e) =>
							   {
								   if (e.ButtonIndex != avAlert.CancelButtonIndex)
								   {
									   positiveAction();
								   }
							   };
				   avAlert.Show();
			   });
		}

		public void ShowOverlay(string text, bool opaque = false)
		{
			HideOverlay();

			view.InvokeOnMainThread(() =>
			{
				// show the loading overlay on the UI thread using the correct orientation sizing
				CoreGraphics.CGRect frame;

				if (view is UIScrollView)
				{
					var scrollView = (UIScrollView)view;
					frame = new CoreGraphics.CGRect(new CoreGraphics.CGPoint(0, scrollView.ContentOffset.Y), UIScreen.MainScreen.Bounds.Size);
				}
				else
				{
					frame = UIScreen.MainScreen.Bounds;
				}

				if (!opaque)
				{
					loadingOverlay = new LoadingOverlay(frame, text);
				}
				else
				{
					view.UserInteractionEnabled = false;
					loadingOverlay = new LoadingOverlay(frame, text, true);
				}

			if (!opaque)
			{
				loadingOverlay = new LoadingOverlay(frame, text);
				view.Add(loadingOverlay);
			}
			else
			{
				view.UserInteractionEnabled = false;
				loadingOverlay = new LoadingOverlay(frame, text, true);
			}

				view.BringSubviewToFront(loadingOverlay);
			});
		}

		public void HideOverlay()
		{
			if (loadingOverlay != null)
			{
				view.InvokeOnMainThread(() =>
				{
					view.UserInteractionEnabled = true;

					loadingOverlay.Hide();
				});

				loadingOverlay = null;

			}
		}
	}

}
