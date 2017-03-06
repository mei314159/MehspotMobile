using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Foundation;
using mehspot.iOS.Extensions;
using UIKit;

namespace mehspot.iOS.Views
{
    [Register ("MaskedTextEditCell")]
    public class MaskedTextEditCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("MaskedTextEditCell");
        public static readonly UINib Nib;

        static MaskedTextEditCell ()
        {
            Nib = UINib.FromName ("MaskedTextEditCell", NSBundle.MainBundle);
        }

        protected MaskedTextEditCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        [Outlet]
        UILabel FieldLabel { get; set; }

        [Outlet]
        UITextField TextInput { get; set; }

        public int? MaxLength { get; set; }
        string mask;

        public string Mask {
            get {
                return mask;
            }

            set {
                mask = value;
                this.TextInput.Placeholder = mask;
            }
        }

        public bool IsValid => ValidateMask (TextInput.Text, true);

        public Action<string> SetModelProperty;
        public event Action<MaskedTextEditCell, string> ValueChanged;

        public static MaskedTextEditCell Create<T> (T Model, Expression<Func<T, string>> property, string placeholder, bool isReadOnly = false) where T : class
        {
            var cell = (MaskedTextEditCell)Nib.Instantiate (null, null) [0];
            cell.FieldLabel.Text = placeholder;
            cell.TextInput.Placeholder = placeholder;
            cell.TextInput.Enabled = !isReadOnly;
            cell.TextInput.Text = property.Compile ().Invoke (Model)?.ToString ();
            cell.TextInput.EditingChanged += cell.TextInput_EditingChanged;
            cell.TextInput.ShouldChangeCharacters += (textField, range, replacementString) => cell.TextInput_ShouldChangeCharacters (textField, range, replacementString);
            cell.SetModelProperty = (text) => { Model.SetProperty (property, text); };
            return cell;
        }

        void TextInput_EditingChanged (object sender, EventArgs e)
        {
            var text = ((UITextField)sender).Text;
            this.SetModelProperty (text);
            this.ValueChanged?.Invoke (this, text);
        }

        bool TextInput_ShouldChangeCharacters (UITextField textField, NSRange range, string replacementString)
        {
            string text = textField.Text;
            string newText = text.Substring (0, (int)range.Location) + replacementString + text.Substring ((int)(range.Location + range.Length));

            if (this.MaxLength.HasValue && newText.Length > this.MaxLength.Value) {
                return false;
            }

            return this.Mask != null ? this.ValidateMask (newText) : true;
        }

        bool ValidateMask (string text, bool fullMatch = false)
        {
            var maskedValue = Regex.Replace (text, "\\d", "#");
            maskedValue = Regex.Replace (maskedValue, "\\w", "*");

            var result = fullMatch ? string.Equals (maskedValue, Mask, StringComparison.InvariantCultureIgnoreCase) : this.Mask.StartsWith (maskedValue, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        void ReleaseDesignerOutlets ()
        {
            if (FieldLabel != null) {
                FieldLabel.Dispose ();
                FieldLabel = null;
            }

            if (TextInput != null) {
                TextInput.Dispose ();
                TextInput = null;
            }
        }
    }
}
