using System;
using System.Linq;
using System.Text.RegularExpressions;
using CoreGraphics;
using Foundation;
using Mehspot.Core.Builders;
using Mehspot.iOS.Extensions;
using UIKit;

namespace Mehspot.iOS.Views
{
	[Register("TextEditCell")]
	public class TextEditCell : UITableViewCell, ITextEditCell
	{
		private const int MaxDigits = 9;

		private string mask;

		public static readonly NSString Key = new NSString("TextEditCell");
		public static readonly UINib Nib;

		static TextEditCell()
		{
			Nib = UINib.FromName("TextEditCell", NSBundle.MainBundle);
		}

		protected TextEditCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		[Outlet]
		public UILabel FieldLabel { get; set; }

		[Outlet]
		public ExtendedTextView TextView { get; set; }

		[Outlet]
		public NSLayoutConstraint TextViewHeight { get; set; }

		public int? MaxLength { get; set; }

		public string Mask
		{
			get
			{
				return mask;
			}

			private set
			{
				mask = value;
				this.MaxLength = mask?.Length;
				this.TextView.Placeholder = mask;
			}
		}

		public string ValidationRegex { get; set; }

		public bool IsValid => ValidateMask(TextView.Text, true);

		public string Text
		{
			get
			{
				return TextView?.Text;
			}
			set
			{
				TextView.Text = value;
			}
		}

		public bool Editable
		{
			get
			{
				return TextView.Editable;
			}

			set
			{
				TextView.Editable = value;
			}
		}

		public bool Multiline
		{
			get
			{
				return TextView.Multiline;
			}

			set
			{
				TextView.Multiline = value;
			}
		}

		public Action<ITextEditCell, string> SetModelProperty;

		public static TextEditCell Create(string initialValue, Action<ITextEditCell, string> setProperty, string label, KeyboardType type = KeyboardType.Default, string placeholder = null, bool isReadOnly = false, string mask = null, string validationRegex = null)
		{
			var cell = (TextEditCell)Nib.Instantiate(null, null)[0];
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			cell.FieldLabel.Text = label;
			if (string.IsNullOrWhiteSpace(mask))
			{
				cell.TextView.Placeholder = placeholder ?? label;
			}
			else
			{
				cell.Mask = mask;
			}
			cell.TextView.Editable = !isReadOnly;
			cell.TextView.Text = initialValue;
			cell.TextView.ShowPlaceholder();
			cell.TextView.Changed += cell.TextInput_EditingChanged;
			cell.TextView.ShouldChangeText += cell.TextInput_ShouldChangeCharacters;
			cell.TextView.ShouldEndEditing += (textView) => { cell.UpdateSize(); return true; };
			cell.SetModelProperty = setProperty;
			cell.SetKeyboardType(type);

			cell.ValidationRegex = validationRegex;
			cell.UpdateSize();
			return cell;
		}


		public void SetKeyboardType(KeyboardType type)
		{
			switch (type)
			{
				case KeyboardType.Decimal:
					TextView.KeyboardType = UIKeyboardType.DecimalPad;
					this.MaxLength = MaxDigits;
					break;
				case KeyboardType.Numeric:
					TextView.KeyboardType = UIKeyboardType.NumberPad;
					this.MaxLength = MaxDigits;
					break;
				case KeyboardType.Email:
					TextView.KeyboardType = UIKeyboardType.EmailAddress;
					break;
				case KeyboardType.Phone:
					TextView.KeyboardType = UIKeyboardType.PhonePad;
					break;
				default:
					TextView.KeyboardType = UIKeyboardType.Default;
					break;
			}
		}


		void TextInput_EditingChanged(object sender, EventArgs e)
		{
			var textView = ((UITextView)sender);
			var text = textView.Text;
			this.SetModelProperty(this, text);
			UpdateSize();
		}

		bool TextInput_ShouldChangeCharacters(UITextView textField, NSRange range, string replacementString)
		{
			string text = textField.Text;
			string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)(range.Location + range.Length));

			if (this.MaxLength.HasValue && newText.Length > this.MaxLength.Value)
				return false;


			if (ValidationRegex != null && !Regex.IsMatch(newText, ValidationRegex))
			{
				return false;
			}

			if (this.Mask == null || this.ValidateMask(newText))
				return true;

			if (text.Length == range.Location && Mask.Length > range.Location)
			{
				var maskSymbol = Mask[(int)range.Location];
				if (maskSymbol != '#' && maskSymbol != '*')
					textField.Text = textField.Text + maskSymbol;
			}

			return false;
		}

		void UpdateSize()
		{
			var sizeThatFitsTextView = this.TextView.SizeThatFits(new CGSize(this.TextView.Frame.Size.Width, int.MaxValue));
			var height = sizeThatFitsTextView.Height > 100 ? 100 : sizeThatFitsTextView.Height < 43 ? 43 : sizeThatFitsTextView.Height;
			TextViewHeight.Constant = height;
			this.TextView.Frame = new CGRect(this.TextView.Frame.Location, new CGSize(this.TextView.Frame.Width, height));
			this.Frame = new CGRect(this.Frame.Location, new CGSize(this.Frame.Width, height + 1));
			var table = this.FindSuperviewOfType(null, typeof(UITableView)) as UITableView;
			if (table != null)
			{
				table.BeginUpdates();
				table.EndUpdates();
			}
		}

		bool ValidateMask(string text, bool fullMatch = false)
		{
			var maskedValue = Regex.Replace(text, "\\d", "#");
			maskedValue = Regex.Replace(maskedValue, "\\w", "*");

			string notAllowedSymbolsRegex;
			var allowedSymbols = Regex.Matches(mask, "[^\\#\\*]");
			if (allowedSymbols.Count > 0)
			{
				notAllowedSymbolsRegex = string.Format("[^\\#\\*\\{0}]", string.Join("\\", allowedSymbols.Cast<Match>().Select(a => a.Value).Distinct()));
			}
			else
			{
				notAllowedSymbolsRegex = "[^\\#\\*]";
			}

			maskedValue = Regex.Replace(maskedValue, notAllowedSymbolsRegex, "_");

			var result = fullMatch ? string.Equals(maskedValue, Mask, StringComparison.InvariantCultureIgnoreCase) : this.Mask.StartsWith(maskedValue, StringComparison.InvariantCultureIgnoreCase);
			return result;
		}

		void ReleaseDesignerOutlets()
		{
			if (FieldLabel != null)
			{
				FieldLabel.Dispose();
				FieldLabel = null;
			}

			if (TextView != null)
			{
				TextView.Dispose();
				TextView = null;
			}
		}
	}
}
