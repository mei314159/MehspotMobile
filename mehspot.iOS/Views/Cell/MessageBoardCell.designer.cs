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
    [Register ("MessageBoardCell")]
    partial class MessageBoardCell
    {
        void ReleaseDesignerOutlets ()
        {
            if (CountLabel != null) {
                CountLabel.Dispose ();
                CountLabel = null;
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