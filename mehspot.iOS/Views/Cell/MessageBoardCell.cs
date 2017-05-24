using System;
using System.CodeDom.Compiler;
using Foundation;
using UIKit;

namespace Mehspot.iOS
{
    [Register ("MessageBoardCell")]
    public class MessageBoardCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("MessageBoardCell");
        public static readonly UINib Nib;

        static MessageBoardCell ()
        {
            Nib = UINib.FromName ("MessageBoardCell", NSBundle.MainBundle);
        }

        [Outlet]
        public UILabel CountLabel { get; set; }

        [Outlet]
        public UILabel Message { get; set; }

        [Outlet]
        public UIImageView ProfilePicture { get; set; }

        [Outlet]
        public UILabel UserName { get; set; }

        protected MessageBoardCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }


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
