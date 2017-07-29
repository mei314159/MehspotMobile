
using System;
using CoreGraphics;
using UIKit;

namespace Mehspot.iOS.Views
{
	public class LoadingOverlay : UIView
	{
		// control declarations
		UIActivityIndicatorView activitySpinner;
		UILabel loadingLabel;

		public LoadingOverlay(CGRect frame, string text, bool opaque = false) : base(frame)
		{
			if (!opaque)
			{
				Initialize(UIColor.FromRGBA(0, 0, 0, 0.8f), UIActivityIndicatorViewStyle.WhiteLarge, text);
			}
			else
			{ 
				Initialize(UIColor.White, UIActivityIndicatorViewStyle.Gray, text);
				activitySpinner.Transform = CGAffineTransform.MakeScale(2f, 2f);
			}
		}

		private void Initialize(UIColor color, UIActivityIndicatorViewStyle style, string text)
		{
			// configurable bits
			BackgroundColor = color;// UIColor.FromRGBA(255, 255, 255, 1.0f);
			AutoresizingMask = UIViewAutoresizing.All;

			nfloat labelHeight = 22;
			nfloat labelWidth = Frame.Width - 20;

			// derive the center x and y
			nfloat centerX = Frame.Width / 2;
			nfloat centerY = Frame.Height / 2;

			// create the activity spinner, center it horizontall and put it 5 points above center x
			activitySpinner = new UIActivityIndicatorView(style);
			activitySpinner.Frame = new CGRect(
				centerX - (activitySpinner.Frame.Width / 2),
							centerY - activitySpinner.Frame.Height - 20,
							activitySpinner.Frame.Width,
							activitySpinner.Frame.Height);
			activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
			AddSubview(activitySpinner);
			activitySpinner.StartAnimating();

			// create and configure the "Loading Data" label
			loadingLabel = new UILabel(new CGRect(
				centerX - (labelWidth / 2),
				centerY + 20,
				labelWidth,
				labelHeight
				));
			loadingLabel.BackgroundColor = UIColor.Clear;
			loadingLabel.TextColor = UIColor.White;
			loadingLabel.Text = text;
			loadingLabel.TextAlignment = UITextAlignment.Center;
			loadingLabel.AutoresizingMask = UIViewAutoresizing.All;
			AddSubview(loadingLabel);
		}

		/// <summary>
		/// Fades out the control and then removes it from the super view
		/// </summary>
		public void Hide()
		{
			UIView.Animate(
				0.5, // duration
				() => { UIColor.FromRGBA(0, 0, 0, 0); },
				() => { RemoveFromSuperview(); }
			);
		}
	}
}
