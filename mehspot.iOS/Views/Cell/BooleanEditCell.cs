using System;
using System.Linq.Expressions;
using Foundation;
using mehspot.iOS.Extensions;
using UIKit;

namespace mehspot.iOS.Views
{
    public partial class BooleanEditCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("BooleanEditCell");
        public static readonly UINib Nib;

        public event Action<bool> ValueChanged;

        static BooleanEditCell ()
        {
            Nib = UINib.FromName ("BooleanEditCell", NSBundle.MainBundle);
        }

        protected BooleanEditCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static BooleanEditCell Create<T> (T Model, Expression<Func<T, bool>> property, string placeholder, bool isReadOnly = false) where T : class
        {
            var cell = (BooleanEditCell)Nib.Instantiate (null, null) [0];
            cell.Switch.Enabled = !isReadOnly;
            cell.FieldLabel.Text = placeholder;
            cell.Switch.SetState (property.Compile ().Invoke (Model), false);

            cell.Switch.ValueChanged += (sender, e) => {
                var value = ((UISwitch)sender).On;
                Model.SetProperty (property, value);
                cell.ValueChanged?.Invoke (value);
            };

            return cell;
        }

        public static BooleanEditCell Create<T> (T Model, Expression<Func<T, bool?>> property, string placeholder, bool isReadOnly = false) where T : class
        {
            var cell = (BooleanEditCell)Nib.Instantiate (null, null) [0];
            cell.Switch.Enabled = !isReadOnly;
            cell.FieldLabel.Text = placeholder;
            cell.Switch.SetState (property.Compile ().Invoke (Model) == true, false);

            cell.Switch.ValueChanged += (sender, e) => {
                Model.SetProperty (property, ((UISwitch)sender).On == true ? true : (bool?)null);
            };

            return cell;
        }
    }
}
