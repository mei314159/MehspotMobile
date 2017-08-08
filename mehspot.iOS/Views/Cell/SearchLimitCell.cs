using System;

using Foundation;
using UIKit;

namespace Mehspot.iOS.Views.Cell
{
	public partial class SearchLimitCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("SearchLimitCell");
		public static readonly UINib Nib;

		static SearchLimitCell()
		{
			Nib = UINib.FromName("SearchLimitCell", NSBundle.MainBundle);
		}

		public event Action OnRegisterButtonTouched;

		public static nfloat Height { get; } = 125;

		protected SearchLimitCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public static SearchLimitCell Create(string description)
		{
			var cell = (SearchLimitCell)Nib.Instantiate(null, null)[0];
			cell.RegisterButton.Layer.BorderWidth = 1;
			cell.RegisterButton.Layer.BorderColor = cell.RegisterButton.TitleColor(UIControlState.Normal).CGColor;
			cell.Message.Text = description;
			return cell;
		}

		partial void RegisterButtonTouched(UIButton sender)
		{
			this.OnRegisterButtonTouched?.Invoke();
		}
	}
}
