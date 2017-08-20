
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.Content;
using Android.Util;
using Android.Widget;

namespace Mehspot.AndroidApp.Views
{
    public class ExtendedEditText : EditText
    {
        private string mask;
        private string previousText;

        public ExtendedEditText(Context context) :
            base(context)
        {
            Initialize();
        }

        public ExtendedEditText(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public ExtendedEditText(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public ExtendedEditText(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
        }

        void Initialize()
        {
        }

		public bool IsValid => ValidateMask(Text, true);

		public bool Multiline
		{
			get
			{
				return this.InputType == Android.Text.InputTypes.TextFlagMultiLine;
			}
			set
			{
				if (value)
				{
					this.SetRawInputType(Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextFlagMultiLine);
					this.SetMinLines(5);
					this.SetMaxLines(100);
				}
				else
				{
					this.SetRawInputType(Android.Text.InputTypes.ClassText);
					this.SetMinLines(1);
					this.SetMaxLines(1);
				}
			}
		}

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
				this.Hint = mask;
			}
		}

		public int? MaxLength { get; set; }

		public string ValidationRegex { get; set; }


		void this_BeforeTextChanged(object sender, Android.Text.TextChangedEventArgs e)
		{
			this.previousText = ((EditText)sender).Text;
		}


        protected override void OnTextChanged(Java.Lang.ICharSequence text, int start, int lengthBefore, int lengthAfter)
        {
			var cursor = this.SelectionStart;
            var txt = new StringBuilder().Append(text).ToString();

			if (string.IsNullOrEmpty(txt))
			{
				return;
			}

			if (this.MaxLength.HasValue && txt.Length > this.MaxLength.Value)
			{
				this.Text = txt.Substring(0, this.MaxLength.Value);
				this.SetSelection(this.Text.Length);
				return;
			}

			if (ValidationRegex != null && !Regex.IsMatch(txt, ValidationRegex))
			{
				this.Text = previousText;
				return;
			}

			if (this.Mask != null && !this.ValidateMask(txt))
			{
				if (Mask.Length > txt.Length)
				{
                    var maskSymbol = Mask[start];
					if (maskSymbol != '#' && maskSymbol != '*')
					{
						this.Text = previousText + maskSymbol;
					}
					else
					{
						this.Text = previousText;
					}
				}

				this.SetSelection(this.Text.Length);
			}

			previousText = this.Text;
        }

		bool ValidateMask(string text, bool fullMatch = false)
		{
			var maskedValue = Regex.Replace(text, "\\d", "#");
			maskedValue = Regex.Replace(maskedValue, "\\w", "*");

			string notAllowedSymbolsRegex;
			var allowedSymbols = Regex.Matches(Mask, "[^\\#\\*]");
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


        public void SetKeyboardType(Mehspot.Core.Builders.KeyboardType type, int? maxLength = null)
		{
			switch (type)
			{
				case Mehspot.Core.Builders.KeyboardType.Decimal:
					this.SetRawInputType(Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagDecimal | Android.Text.InputTypes.NumberFlagSigned);
					this.MaxLength = maxLength;
					break;
				case Mehspot.Core.Builders.KeyboardType.Numeric:
					this.SetRawInputType(Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberVariationNormal | Android.Text.InputTypes.NumberFlagSigned);
					this.MaxLength = maxLength;
					break;
				case Mehspot.Core.Builders.KeyboardType.Phone:
					this.SetRawInputType(Android.Text.InputTypes.ClassPhone | Android.Text.InputTypes.NumberVariationNormal | Android.Text.InputTypes.NumberFlagSigned);
					break;
				case Mehspot.Core.Builders.KeyboardType.Email:
					this.SetRawInputType(Android.Text.InputTypes.ClassText | Android.Text.InputTypes.NumberVariationNormal | Android.Text.InputTypes.NumberFlagSigned);
					break;
				default:
					this.SetRawInputType(Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationNormal);
					break;
			}
		}
	}
}
