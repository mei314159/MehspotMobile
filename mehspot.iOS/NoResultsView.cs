using Foundation;
using System;
using UIKit;

namespace mehspot.iOS
{
	public partial class NoResultsView : UIView
	{
		public static readonly NSString Key = new NSString("NoResultsView");
		public static readonly UINib Nib;

		static NoResultsView()
		{
			Nib = UINib.FromName("NoResultsView", NSBundle.MainBundle);
		}

		public NoResultsView(IntPtr handle) : base(handle)
		{
		}

		public event Action OnRegisterButtonTouched;

		public static NoResultsView Create(string description)
		{
			var cell = (NoResultsView)Nib.Instantiate(null, null)[0];
			cell.RegisterButton.Layer.BorderWidth = 1;
			cell.RegisterButton.Layer.BorderColor = cell.RegisterButton.TitleColor(UIControlState.Normal).CGColor;
			cell.Message.Text = description;
			return cell;
		}

		partial void RegisterButtonTouched(UIButton sender)
		{
			this.OnRegisterButtonTouched?.Invoke();
		}

		public void HideRegisterButton()
		{
			RegisterButton.Hidden = true;
			Message.Hidden = true;
		}

		public void ShowRegisterButton()
		{
			RegisterButton.Hidden = false;
			Message.Hidden = false;
		}

	}
}