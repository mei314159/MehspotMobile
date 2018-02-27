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
    [Register ("SearchResultsViewController")]
    partial class SearchResultsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView ActivityIndicator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem MostActiveButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationItem NavBar { get; set; }

        [Action ("MostActiveButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void MostActiveButton_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (ActivityIndicator != null) {
                ActivityIndicator.Dispose ();
                ActivityIndicator = null;
            }

            if (MostActiveButton != null) {
                MostActiveButton.Dispose ();
                MostActiveButton = null;
            }

            if (NavBar != null) {
                NavBar.Dispose ();
                NavBar = null;
            }
        }
    }
}