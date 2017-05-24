using System;

using Foundation;
using UIKit;

namespace Mehspot.iOS.Views.Cell
{
    public partial class ButtonCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("ButtonCell");
        public static readonly UINib Nib;

        public event Action<ButtonCell, UIButton> OnButtonTouched;

        static ButtonCell ()
        {
            Nib = UINib.FromName ("ButtonCell", NSBundle.MainBundle);
        }

        protected ButtonCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        internal static ButtonCell Create (string title)
        {
            var cell = (ButtonCell)Nib.Instantiate (null, null) [0];
            cell.Button.SetTitle (title, UIControlState.Normal);
            return cell;

        }

        partial void ButtonTouched (UIButton sender)
        {
            OnButtonTouched?.Invoke (this, sender);
        }
    }
}
