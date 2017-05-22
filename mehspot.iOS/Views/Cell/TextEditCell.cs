using System;
using System.Linq;
using System.Text.RegularExpressions;
using Foundation;
using UIKit;

namespace mehspot.iOS.Views
{
    [Register ("TextEditCell")]
    public class TextEditCell : UITableViewCell
    {
        private string mask;
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

        [Outlet]
        UILabel FieldLabel { get; set; }

        [Outlet]
        public UITextField TextInput { get; set; }


        public int? MaxLength { get; set; }

        public string Mask {
            get {
                return mask;
            }

            set {
                mask = value;
                this.MaxLength = mask?.Length;
                this.TextInput.Placeholder = mask;
            }
        }

        public string ValidationRegex { get; set; }

        public bool IsValid => ValidateMask (TextInput.Text, true);

        public Action<string> SetModelProperty;
        public event Action<TextEditCell, string> ValueChanged;

		public static TextEditCell Create(string initialValue, Action<string> setProperty, string label, string placeholder = null, bool isReadOnly = false, string mask = null)
        {
            var cell = (TextEditCell)Nib.Instantiate (null, null) [0];
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.FieldLabel.Text = label;
            cell.TextInput.Placeholder = placeholder ?? label;
            cell.TextInput.Enabled = !isReadOnly;
            cell.TextInput.Text = initialValue;
            cell.TextInput.EditingChanged += cell.TextInput_EditingChanged;
            cell.TextInput.ShouldChangeCharacters += (textField, range, replacementString) => cell.TextInput_ShouldChangeCharacters (textField, range, replacementString);
            cell.SetModelProperty = setProperty;
			cell.Mask = mask;
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

            if (this.MaxLength.HasValue && newText.Length > this.MaxLength.Value)
                return false;


            if (ValidationRegex != null && !Regex.IsMatch (newText, ValidationRegex)) {
                return false;
            }

            if (this.Mask == null || this.ValidateMask (newText))
                return true;

            if (text.Length == range.Location && Mask.Length > range.Location) {
                var maskSymbol = Mask [(int)range.Location];
                if (maskSymbol != '#' && maskSymbol != '*')
                    textField.Text = textField.Text + maskSymbol;
            }

            return false;
        }

        bool ValidateMask (string text, bool fullMatch = false)
        {
            var maskedValue = Regex.Replace (text, "\\d", "#");
            maskedValue = Regex.Replace (maskedValue, "\\w", "*");

            string notAllowedSymbolsRegex;
            var allowedSymbols = Regex.Matches (mask, "[^\\#\\*]");
            if (allowedSymbols.Count > 0) {
                notAllowedSymbolsRegex = string.Format ("[^\\#\\*\\{0}]", string.Join ("\\", allowedSymbols.Cast<Match> ().Select (a => a.Value).Distinct ()));
            } else {
                notAllowedSymbolsRegex = "[^\\#\\*]";
            }

            maskedValue = Regex.Replace (maskedValue, notAllowedSymbolsRegex, "_");

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
