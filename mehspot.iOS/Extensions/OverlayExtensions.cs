using System.Collections.Generic;
using CoreGraphics;
using UIKit;

namespace Mehspot.iOS.Extensions
{

	public static class OverlayExtensions
	{
		public static MehOverlay CreateOverlay(this UIView target)
		{
			var overlay = new MehOverlay(target);
			return overlay;
		}
	}

	public class MehOverlay : UIView
	{
		readonly List<CGRect> holesArray = new List<CGRect>();
		readonly List<CGRect> squareHolesArray = new List<CGRect>();

		public MehOverlay(UIView target) : base(target.Bounds)
		{
			Target = target;
			this.Frame = target.Bounds;
		}

		public UIView Target { get; set; }

		public MehOverlay Show()
		{
			UIView superview;
			if (Target.Superview != null)
			{
				superview = Target.Superview;
				this.Frame = Target.Frame;
			}
			else
			{
				superview = Target;
				this.Frame = Target.Bounds;
			}

			superview.AddSubview(this);
			superview.BringSubviewToFront(this);
			return this;
		}

		public void Hide()
		{
			if (Superview != null)
			{
				this.RemoveFromSuperview();
			}
		}

		public MehOverlay SetBackgroundColor(UIColor color)
		{
			this.BackgroundColor = color;
			return this;
		}

		public MehOverlay HighlightCircle(UIView view)
		{
			var frame = Target.ConvertRectFromView(view.Bounds, view);
			this.holesArray.Add(frame);
			return this;
		}

		public MehOverlay HighlightSquare(UIView view)
		{
			var frame = Target.ConvertRectFromView(view.Bounds, view);
			this.squareHolesArray.Add(frame);
			return this;
		}


		public void AddLabel(UILabel label)
		{
			this.AddSubview(label);
		}

		public void AddImage(UIImage image, CGRect frame)
		{
			var uIImageView = new UIImageView(image);
			uIImageView.Frame = frame;
			this.AddSubview(uIImageView);
		}

		public override void Draw(CGRect rect)
		{
			var gctx = UIGraphics.GetCurrentContext();

			gctx.SetFillColor(BackgroundColor.CGColor);
			gctx.FillRect(rect);

			gctx.SetBlendMode(CGBlendMode.Clear);
			UIColor.Clear.SetColor();

			foreach (var holeRectValue in holesArray)
			{
				CGRect holeRectIntersection = CGRect.Intersect(holeRectValue, rect);
				gctx.AddEllipseInRect(holeRectIntersection);
			}

			foreach (var holeRectValue in squareHolesArray)
			{
				CGRect holeRectIntersection = CGRect.Intersect(holeRectValue, rect);
				gctx.AddRect(holeRectIntersection);
			}

			gctx.DrawPath(CGPathDrawingMode.Fill);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (var item in Subviews)
				{
					item.Dispose();
				}
			}

			base.Dispose(disposing);
		}
	}
}
