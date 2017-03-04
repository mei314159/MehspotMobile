using System;
using System.Linq.Expressions;
using Foundation;
using mehspot.iOS.Extensions;
using ObjCRuntime;
using UIKit;

namespace mehspot.iOS.Views
{
    public partial class TextEditCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("TextEditCell");
        public static readonly UINib Nib;

        static TextEditCell ()
        {
            Nib = UINib.FromName ("TextEditCell", NSBundle.MainBundle);
        }

        protected TextEditCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static TextEditCell Create<T> (T Model, Expression<Func<T, object>> property, string placeholder, bool isReadOnly = false) where T : class
        {
            var cell = (TextEditCell)Nib.Instantiate (null, null)[0];
            cell.TextInput.Placeholder = placeholder;
            cell.TextInput.Enabled = !isReadOnly;
            cell.FieldLabel.Text = placeholder;
            cell.TextInput.Text = property.Compile ().Invoke (Model)?.ToString();

            cell.TextInput.EditingChanged += (sender, e) => {
                Model.SetProperty (property, ((UITextField)sender).Text);
            };

            return cell;
        }
    }
}
