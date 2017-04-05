using System;
using System.Text.RegularExpressions;
using Foundation;
using UIKit;

namespace mehspot.iOS.Views
{
    [Register ("MultilineTextEditCell")]
    public class MultilineTextEditCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("MultilineTextEditCell");
        public static readonly UINib Nib;

        static MultilineTextEditCell ()
        {
            Nib = UINib.FromName ("MultilineTextEditCell", NSBundle.MainBundle);
        }

        protected MultilineTextEditCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        [Outlet]
        UILabel FieldLabel { get; set; }

        [Outlet]
        public UITextView TextInput { get; set; }


        public int? MaxLength { get; set; }

        public string ValidationRegex { get; set; }

        public Action<string> SetModelProperty;
        public event Action<MultilineTextEditCell, string> ValueChanged;

        public static MultilineTextEditCell Create (string initialValue, Action<string> setValue, string label, bool isReadOnly = false)
        {
            var cell = (MultilineTextEditCell)Nib.Instantiate (null, null) [0];
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.FieldLabel.Text = label;
            cell.TextInput.Editable = !isReadOnly;
            cell.TextInput.Text = initialValue;
            cell.TextInput.Changed += cell.TextInput_EditingChanged;
            cell.TextInput.ShouldChangeText += (textField, range, replacementString) => cell.TextInput_ShouldChangeCharacters (textField, range, replacementString);
            cell.SetModelProperty = setValue;
            return cell;
        }

        void TextInput_EditingChanged (object sender, EventArgs e)
        {
            var text = ((UITextView)sender).Text;
            this.SetModelProperty (text);
            this.ValueChanged?.Invoke (this, text);
        }

        bool TextInput_ShouldChangeCharacters (UITextView textField, NSRange range, string replacementString)
        {
            string text = textField.Text;
            string newText = text.Substring (0, (int)range.Location) + replacementString + text.Substring ((int)(range.Location + range.Length));

            if (this.MaxLength.HasValue && newText.Length > this.MaxLength.Value)
                return false;


            if (ValidationRegex != null && !Regex.IsMatch (newText, ValidationRegex)) {
                return false;
            }

            return true;
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
