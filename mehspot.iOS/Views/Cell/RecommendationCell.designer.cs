// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Mehspot.iOS.Views.Cell
{
	[Register ("RecommendationCell")]
	partial class RecommendationCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel DateField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel Message { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIImageView ProfilePicture { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint RecommendationHeight { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel UserName { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (RecommendationHeight != null) {
				RecommendationHeight.Dispose ();
				RecommendationHeight = null;
			}

			if (DateField != null) {
				DateField.Dispose ();
				DateField = null;
			}

			if (Message != null) {
				Message.Dispose ();
				Message = null;
			}

			if (ProfilePicture != null) {
				ProfilePicture.Dispose ();
				ProfilePicture = null;
			}

			if (UserName != null) {
				UserName.Dispose ();
				UserName = null;
			}
		}
	}
}
