using System;
using System.Linq;
using System.Text.RegularExpressions;
using Android.Content;
using Android.Views;
using Android.Widget;
using Mehspot.Core.Builders;

namespace Mehspot.AndroidApp
{

	public class TextEditCell : RelativeLayout, ITextEditCell
	{
		string previousText;

		private string mask;
		public Action<ITextEditCell, string> SetModelProperty;
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

		public TextEditCell(Context context, string initialValue, Action<ITextEditCell, string> setProperty, string label, string placeholder = null, bool isReadOnly = false, string mask = null, string validationRegex = null) : base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.TextEditCell, this);

			FieldLabel.Text = label;
			this.TextInput.Hint = placeholder ?? label;
			this.TextInput.Enabled = !isReadOnly;
			this.TextInput.Text = initialValue;
			this.TextInput.TextChanged += TextInput_TextChanged;
			this.TextInput.BeforeTextChanged += TextInput_BeforeTextChanged;
			this.SetModelProperty = setProperty;
			this.Mask = mask;
			this.ValidationRegex = validationRegex;
		}

		public TextView FieldLabel => this.FindViewById<TextView>(Resource.TextEditCell.FieldLabel);

		public EditText TextInput => this.FindViewById<EditText>(Resource.TextEditCell.TextInput);

		public bool Multiline
		{
			get
			{
				return TextInput.InputType == Android.Text.InputTypes.TextFlagMultiLine;
			}
			set
			{
				if (value)
				{
					this.TextInput.SetRawInputType(Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextFlagMultiLine);
					TextInput.SetMinLines(5);
					TextInput.SetMaxLines(100);
				}
				else
				{
					this.TextInput.SetRawInputType(Android.Text.InputTypes.ClassText);
					TextInput.SetMinLines(1);
					TextInput.SetMaxLines(1);
				}
			}
		}

		public bool Hidden
		{
			get
			{
				return this.Visibility == ViewStates.Gone;
			}

			set
			{
				this.Visibility = value ? ViewStates.Gone : ViewStates.Visible;
			}
		}

		public string Text
		{
			get
			{
				return this.TextInput?.Text;
			}
			set
			{
				this.TextInput.Text = value;
			}
		}

		public bool Editable
		{
			get
			{
				return this.TextInput.Enabled;
			}

			set
			{
				this.TextInput.Enabled = value;
			}
		}

		void TextInput_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
		{
			var text = TextInput.Text;

			if (this.MaxLength.HasValue && text.Length > this.MaxLength.Value)
			{
				TextInput.Text = text.Substring(0, this.MaxLength.Value);
				return;
			}


			if (ValidationRegex != null && !Regex.IsMatch(text, ValidationRegex))
			{
				TextInput.Text = previousText;
				return;
			}


			if (this.Mask != null && !this.ValidateMask(text))
			{
				if (Mask.Length > text.Length)
				{
					var maskSymbol = Mask[text.Length];
					if (maskSymbol != '#' && maskSymbol != '*')
					{
						TextInput.Text = text + maskSymbol;
					}
					else
					{
						TextInput.Text = previousText;
					}
				}
			}

			this.SetModelProperty(this, TextInput.Text);
			this.ValueChanged?.Invoke(this, TextInput.Text);
			previousText = TextInput.Text;
		}

		void TextInput_BeforeTextChanged(object sender, Android.Text.TextChangedEventArgs e)
		{
			this.previousText = ((EditText)sender).Text;
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

		public void SetKeyboardType(Mehspot.Core.Builders.KeyboardType type)
		{
			switch (type)
			{
				case Mehspot.Core.Builders.KeyboardType.Decimal:
					this.TextInput.SetRawInputType(Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagDecimal | Android.Text.InputTypes.NumberFlagSigned);
					break;
				case Mehspot.Core.Builders.KeyboardType.Numeric:
					this.TextInput.SetRawInputType(Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberVariationNormal | Android.Text.InputTypes.NumberFlagSigned);
					break;
				default:
					this.TextInput.SetRawInputType(Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationNormal);
					break;
			}
		}
	}

}
