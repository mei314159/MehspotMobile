using System;
using System.Linq;
using CoreGraphics;
using UIKit;

namespace mehspot.iOS.Views
{
    public class ErrorView : UILabel
    {

        ErrorView(CGRect frame) : base(frame)
        {

        }

        public static ErrorView GetExistingAlert(UIView targetView)
        {
            return targetView.Subviews.OfType<ErrorView>().FirstOrDefault();
        }


        public static ErrorView ShowInView(UIView targetView, CGPoint location, string text, bool autoHide = true)
        {
            var alert = GetExistingAlert(targetView);
            if (alert != null)
            {
                return alert;
            }

            var visibleFrame = new CGRect(location, new CGSize(targetView.Frame.Width, 44));
            var invisibleFrame = new CGRect(location, new CGSize(targetView.Frame.Width, 0));
            alert = new ErrorView(invisibleFrame);
            alert.BackgroundColor = UIColor.FromRGB(249, 77, 89);
            alert.TextColor = UIColor.White;
            alert.Font = UIFont.SystemFontOfSize(16f);
            alert.Text = text;
            alert.TextAlignment = UITextAlignment.Center;
            targetView.AddSubview(alert);

            Animate(0.3, 0, UIViewAnimationOptions.CurveLinear, () =>
            {
                alert.Frame = visibleFrame;
            }, () =>
            {
                if (autoHide)
                {
                    alert.Hide(3);
                }
            });
            return alert;
        }

        public void Hide(double delay = 0)
        {
            Animate(0.3, delay, UIViewAnimationOptions.CurveLinear, () =>
            {
                var invisibleFrame = new CGRect(Frame.Location, new CGSize(Superview.Frame.Width, 0));
                Frame = invisibleFrame;
            }, () =>
            {
                RemoveFromSuperview();
                Dispose();
            });
        }
    }
}
