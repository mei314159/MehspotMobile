using Foundation;
using System;
using UIKit;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Linq;

namespace Mehspot.iOS
{

	[Register("ExtendedTextField"), DesignTimeVisible(true)]
	public class ExtendedTextField : UITextField
	{
		private string mask;

		public string ValidationRegex { get; set; }

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
				this.Placeholder = mask;
			}
		}

		public int? MaxLength { get; set; }

		public bool IsValid => ValidateMask(Text, true);

		public ExtendedTextField(IntPtr handle)
				: base(handle)
		{
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			Initialize();
		}

		void Initialize()
		{
			EnablesReturnKeyAutomatically = true;
			this.ShouldChangeCharacters += Handle_ShouldChangeCharacters;
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

		bool Handle_ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
		{
			string text = Text;
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
					Text = Text + maskSymbol;
			}

			return false;
		}
	}
}