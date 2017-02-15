using System;

using Foundation;
using UIKit;

namespace mehspot.iOS
{
    public partial class MessageBoardCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("MessageBoardCell");
        public static readonly UINib Nib;

        static MessageBoardCell ()
        {
            Nib = UINib.FromName ("MessageBoardCell", NSBundle.MainBundle);
        }

        protected MessageBoardCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
