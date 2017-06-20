﻿using System;
using Mehspot.iOS.Views;
using Mehspot.Core.Contracts.Wrappers;
using UIKit;

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
			var avAlert = new UIAlertView(title, text, (IUIAlertViewDelegate)null, "OK", null);
			avAlert.Show();
		}

		public void ShowPrompt(string title, string text, string positiveButtonTitle, Action positiveAction)
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
		}

		public void ShowOverlay(string text)
		{
			HideOverlay();
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

			loadingOverlay = new LoadingOverlay(frame, text);
			view.Add(loadingOverlay);
			view.UserInteractionEnabled = false;
			view.BringSubviewToFront(loadingOverlay);
		}

		public void HideOverlay()
		{
			if (loadingOverlay != null)
			{
				view.UserInteractionEnabled = true;
				loadingOverlay.Hide();
				loadingOverlay.Dispose();
				loadingOverlay = null;
			}
		}
	}

}
