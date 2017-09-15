// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Mehspot.iOS
{
    [Register ("SearchBadgeController")]
    partial class SearchBadgeController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationItem NavBar { get; set; }

        [Action ("SearchButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SearchButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (NavBar != null) {
                NavBar.Dispose ();
                NavBar = null;
            }
        }
    }
}