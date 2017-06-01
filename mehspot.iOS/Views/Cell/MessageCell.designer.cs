// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Mehspot.iOS
{
    [Register ("MessageCell")]
    partial class MessageCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel message { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView messageWrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint XConstraint { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (message != null) {
                message.Dispose ();
                message = null;
            }

            if (messageWrapper != null) {
                messageWrapper.Dispose ();
                messageWrapper = null;
            }

            if (XConstraint != null) {
                XConstraint.Dispose ();
                XConstraint = null;
            }
        }
    }
}