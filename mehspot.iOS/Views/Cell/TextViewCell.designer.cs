// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;
using UIKit;

namespace Mehspot.iOS.Views
{

	[Register("TextViewCell")]
	partial class TextViewCell
	{
		[Outlet]
		NSLayoutConstraint TextViewHeight { get; set; }

		[Outlet]
		UILabel FieldLabel { get; set; }

		[Outlet]
		UITextView Text { get; set; }

		void ReleaseDesignerOutlets()
		{
			if (FieldLabel != null)
			{
				FieldLabel.Dispose();
				FieldLabel = null;
			}

			if (Text != null)
			{
				Text.Dispose();
				Text = null;
			}

			if (TextViewHeight != null)
			{
				TextViewHeight.Dispose();
				TextViewHeight = null;
			}
		}
	}
}
