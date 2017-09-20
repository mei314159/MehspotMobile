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

namespace Mehspot.iOS.Views
{
    [Register ("TextViewCell")]
    partial class TextViewCell
    {
        [Outlet]
        NSLayoutConstraint TextViewHeight { get; set; }


        [Outlet]
        UILabel FieldLabel { get; set; }


        [Outlet]
        UITextView Text { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (FieldLabel != null) {
                FieldLabel.Dispose ();
                FieldLabel = null;
            }

            if (Text != null) {
                Text.Dispose ();
                Text = null;
            }

            if (TextViewHeight != null) {
                TextViewHeight.Dispose ();
                TextViewHeight = null;
            }
        }
    }
}