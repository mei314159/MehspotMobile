using System;
using System.Linq;
using System.Text.RegularExpressions;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace Mehspot.AndroidApp
{

	public class TextEditCell : RelativeLayout
	{
		private string mask;
		public Action<string> SetModelProperty;
		public event Action<TextEditCell, string> ValueChanged;
		public int? MaxLength { get; set; }

		public string Mask
		{
			get
			{
				return mask;
			}

			set
			{
				mask = value;
				this.MaxLength = mask?.Length;
				this.TextInput.Hint = mask;
			}
		}

		public string ValidationRegex { get; set; }

		public bool IsValid => ValidateMask(TextInput.Text, true);

		public TextEditCell(Context context, string initialValue, Action<string> setProperty, string label, string placeholder = null, bool isReadOnly = false, string mask = null) : base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.TextEditCell, this);

			FieldLabel.Text = label;
			this.TextInput.Hint = placeholder ?? label;
			this.TextInput.Enabled = !isReadOnly;
			this.TextInput.Text = initialValue;
			this.TextInput.TextChanged += TextInput_TextChanged; ;
			this.TextInput.BeforeTextChanged += TextInput_BeforeTextChanged;
			this.SetModelProperty = setProperty;
			this.Mask = mask;
		}

		public TextView FieldLabel => this.FindViewById<TextView>(Resource.TextEditCell.FieldLabel);

		public EditText TextInput => this.FindViewById<EditText>(Resource.TextEditCell.TextInput);

		void TextInput_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
		{
			var text = ((EditText)sender).Text;
			this.SetModelProperty(text);
			this.ValueChanged?.Invoke(this, text);
		}

		void TextInput_BeforeTextChanged(object sender, Android.Text.TextChangedEventArgs e)
		{
			//string text = TextInput.Text;
			//string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)(range.Location + range.Length));

			//if (this.MaxLength.HasValue && newText.Length > this.MaxLength.Value)
			//	return;

			//if (ValidationRegex != null && !Regex.IsMatch(newText, ValidationRegex))
			//{
			//	return;
			//}

			//if (this.Mask == null || this.ValidateMask(newText))
			//	return true;

			//if (text.Length == range.Location && Mask.Length > range.Location)
			//{
			//	var maskSymbol = Mask[(int)range.Location];
			//	if (maskSymbol != '#' && maskSymbol != '*')
			//		TextInput.Text = TextInput.Text + maskSymbol;
			//}

			//return;
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

	}

}
