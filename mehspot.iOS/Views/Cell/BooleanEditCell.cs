using System;
using Foundation;
using UIKit;

namespace mehspot.iOS.Views
{
    public partial class BooleanEditCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("BooleanEditCell");
        public static readonly UINib Nib;

        static BooleanEditCell ()
        {
            Nib = UINib.FromName ("BooleanEditCell", NSBundle.MainBundle);
        }

        protected BooleanEditCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static BooleanEditCell Create (bool initialValue, Action<bool> setValue, string placeholder, bool isReadOnly = false)
        {
            var cell = (BooleanEditCell)Nib.Instantiate (null, null) [0];
            cell.Switch.Enabled = !isReadOnly;
            cell.FieldLabel.Text = placeholder;
            cell.Switch.SetState (initialValue, false);
            cell.Switch.ValueChanged += (sender, e) => setValue (((UISwitch)sender).On);

            return cell;
        }
    }
}
