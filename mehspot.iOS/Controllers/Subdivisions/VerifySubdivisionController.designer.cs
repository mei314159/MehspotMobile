// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Mehspot.iOS.Controllers
{
	[Register ("VerifySubdivisionController")]
	partial class VerifySubdivisionController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITableView MainTable { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint MainTableHeight { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView MapWrapperView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UINavigationBar NavBar { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UINavigationItem NavBarItem { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIBarButtonItem SaveButton { get; set; }

		[Action ("CloseButtonTouched:")]
		partial void CloseButtonTouched (UIKit.UIBarButtonItem sender);

		[Action ("SaveButtonTouched:")]
		partial void SaveButtonTouched (UIKit.UIBarButtonItem sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (MainTableHeight != null) {
				MainTableHeight.Dispose ();
				MainTableHeight = null;
			}

			if (MainTable != null) {
				MainTable.Dispose ();
				MainTable = null;
			}

			if (MapWrapperView != null) {
				MapWrapperView.Dispose ();
				MapWrapperView = null;
			}

			if (NavBar != null) {
				NavBar.Dispose ();
				NavBar = null;
			}

			if (NavBarItem != null) {
				NavBarItem.Dispose ();
				NavBarItem = null;
			}

			if (SaveButton != null) {
				SaveButton.Dispose ();
				SaveButton = null;
			}
		}
	}
}
