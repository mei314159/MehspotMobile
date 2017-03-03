// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace mehspot.iOS
{
    [Register ("EditProfileController")]
    partial class EditProfileController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView ProfileTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ProfileTableView != null) {
                ProfileTableView.Dispose ();
                ProfileTableView = null;
            }
        }
    }
}