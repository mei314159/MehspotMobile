using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace mehspot.iOS.Views.GrouppingView
{
	[Register("DelimiterView"), DesignTimeVisible(true)]
	public class DelimiterView : UIView, IComponent
	{
		public DelimiterView()
		{
		}

		public DelimiterView(Foundation.NSCoder coder) : base(coder)
		{
		}

		public DelimiterView(Foundation.NSObjectFlag t) : base(t)
		{
		}

		public DelimiterView(IntPtr handle) : base(handle)
		{
		}

		public DelimiterView(CoreGraphics.CGRect frame) : base(frame)
		{
		}

		UIColor color = UIColor.FromRGBA(0.984f, 0.447f, 0.000f, 1.000f);

		public event EventHandler Disposed;

		public ISite Site { get; set; }

		[Export("Color"), Browsable(true)]
		public UIColor Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
				SetNeedsDisplay();
			}
		}

		public override void Draw(CoreGraphics.CGRect rect)
		{
			DrawCanvas(rect, this.Color, 20, 3);
		}

		private void DrawCanvas(CGRect frame, UIColor color, nfloat square, nfloat weight)
		{
			//// General Declarations
			var context = UIGraphics.GetCurrentContext();

			//// Variable Declarations
			var whiteW = frame.Width;
			var hypotenusa = NMath.Sqrt(NMath.Pow(square, 2.0f) + NMath.Pow(square, 2.0f));
			var y = hypotenusa / 2.0f;
			var lineWidth = (frame.Width - hypotenusa) / 2.0f + weight / 2.0f;
			var squareX = lineWidth - weight / 2.0f;
			var line2X = lineWidth + hypotenusa - weight;
			var whiteY = y + weight;

			//// Rectangle Drawing
			var rectanglePath = UIBezierPath.FromRect(new CGRect(0.0f, y, lineWidth, weight));
			color.SetFill();
			rectanglePath.Fill();


			//// Rectangle 3 Drawing
			context.SaveState();
			context.TranslateCTM(squareX, (whiteY + 0.0000239668262836f));
			context.RotateCTM(-45.0f * NMath.PI / 180.0f);

			var rectangle3Path = UIBezierPath.FromRect(new CGRect(0.0f, 0.0f, square, square));
			color.SetStroke();
			rectangle3Path.LineWidth = weight;
			rectangle3Path.Stroke();

			context.RestoreState();


			//// Rectangle 2 Drawing
			var rectangle2Path = UIBezierPath.FromRect(new CGRect(line2X, y, lineWidth, weight));
			color.SetFill();
			rectangle2Path.Fill();


			//// Rectangle 4 Drawing
			var rectangle4Path = UIBezierPath.FromRect(new CGRect(0.0f, whiteY, whiteW, hypotenusa));
			UIColor.White.SetFill();
			rectangle4Path.Fill();
		}


	}
}
