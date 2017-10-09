// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace mehspot.iOS.Views.Cell
{
    [Register ("BadgeCategoryCell")]
    partial class BadgeCategoryCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView BadgeCircle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel BadgeDescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel BadgeLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BadgeCircle != null) {
                BadgeCircle.Dispose ();
                BadgeCircle = null;
            }

            if (BadgeDescription != null) {
                BadgeDescription.Dispose ();
                BadgeDescription = null;
            }

            if (BadgeLabel != null) {
                BadgeLabel.Dispose ();
                BadgeLabel = null;
            }
        }
    }
}