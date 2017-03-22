using System;

using Foundation;
using UIKit;

namespace mehspot.iOS.Views.Cell
{
    [Register ("ButtonCell")]
    public class ButtonCell : UITableViewCell
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

        [Outlet]
        public UIKit.UIButton CellButton { get; set; }

        public static ButtonCell Create(Action<UIButton> buttonTouched, string buttonLabel, bool isReadOnly = false)
        {
            var cell = (ButtonCell)Nib.Instantiate (null, null) [0];
            cell.CellButton.Enabled = !isReadOnly;
            cell.CellButton.SetTitle (buttonLabel, UIControlState.Normal);
            cell.CellButton.TouchUpInside += (sender, e) => buttonTouched ((UIButton)sender);
            return cell;
        }

        void ReleaseDesignerOutlets ()
        {
            if (CellButton != null) {
                CellButton.Dispose ();
                CellButton = null;
            }

            if (CellButton != null) {
                CellButton.Dispose ();
                CellButton = null;
            }
        }
    }
}
