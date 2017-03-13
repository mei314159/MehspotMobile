using System;

using Foundation;
using UIKit;

namespace mehspot.iOS.Views.Cell
{
    public partial class ButtonCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("ButtonCell");
        public static readonly UINib Nib;

        static ButtonCell ()
        {
            Nib = UINib.FromName ("ButtonCell", NSBundle.MainBundle);
        }

        protected ButtonCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
