using System;

using Foundation;
using Mehspot.Core;
using ObjCRuntime;
using UIKit;
using Mehspot.Core.Services.Badges;

namespace Mehspot.iOS.Views
{
	[Register("BadgeItemCell")]
	public class BadgeItemCell : UITableViewCell
	{
		bool buttonsVisible;
		public static readonly NSString Key = new NSString("BadgeItemCell");
		public static readonly UINib Nib;

		static BadgeItemCell()
		{
			Nib = UINib.FromName("BadgeItemCell", NSBundle.MainBundle);
		}

		protected BadgeItemCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public BadgeInfo BadgeInfo { get; set; }


		[Outlet]
		UIKit.UILabel BadgeDescription { get; set; }

		[Outlet]
		public UILabel BadgeName { get; set; }

		[Outlet]
		public UIImageView BadgePicture { get; set; }

		[Outlet]
		UIKit.UIButton BadgeRegisterButton { get; set; }

		[Outlet]
		UIKit.UIButton SearchButton { get; set; }

		[Outlet]
		UIKit.UIView ButtonsWrapper { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ButtonsWrapperWidth { get; set; }

		public Action<UIButton> SearchButtonTouch { get; set; }
		public Action<UIButton> BadgeRegisterButtonTouch { get; set; }

		public static BadgeItemCell Create()
		{
			var arr = NSBundle.MainBundle.LoadNib("BadgeItemCell", null, null);
			var v = Runtime.GetNSObject<BadgeItemCell>(arr.ValueAt(0));
			return v;
		}

		[Action("SearchButtonTouched:")]
		void SearchButtonTouched(UIButton sender)
		{
			SearchButtonTouch?.Invoke(sender);
		}

		[Action("BadgeRegisterButtonTouched:")]
		void BadgeRegisterButtonTouched(UIButton sender)
		{
			BadgeRegisterButtonTouch?.Invoke(sender);
		}

		public override void SetSelected(bool selected, bool animated)
		{
			buttonsVisible = selected && !this.buttonsVisible;
			ButtonsWrapperWidth.Constant = buttonsVisible ? 160 : 0;
			UIView.Animate(0.2, () =>
			{
				LayoutIfNeeded();
			});
		}

		public void Initialize(BadgeInfo badge)
		{
			var cell = this;
			cell.BadgePicture.Image = UIImage.FromFile("badges/" + badge.BadgeName.ToLower() + (badge.Badge.IsRegistered ? string.Empty : "b"));
			cell.BadgeName.Text = badge.CustomLabel ?? MehspotResources.ResourceManager.GetString("Find_" + badge.SearchBadge);
			cell.BadgeInfo = badge;
			cell.BadgeRegisterButton.SetTitle(badge.Badge.IsRegistered ? "Update" : "Register", UIControlState.Normal);
			cell.BadgeDescription.Text = MehspotResources.ResourceManager.GetString(badge.BadgeName + "_Description");
		}

		void ReleaseDesignerOutlets()
		{
			if (BadgeDescription != null)
			{
				BadgeDescription.Dispose();
				BadgeDescription = null;
			}

			if (BadgeName != null)
			{
				BadgeName.Dispose();
				BadgeName = null;
			}

			if (BadgePicture != null)
			{
				BadgePicture.Dispose();
				BadgePicture = null;
			}

			if (BadgeRegisterButton != null)
			{
				BadgeRegisterButton.Dispose();
				BadgeRegisterButton = null;
			}

			if (ButtonsWrapperWidth != null)
			{
				ButtonsWrapperWidth.Dispose();
				ButtonsWrapperWidth = null;
			}

			if (ButtonsWrapper != null)
			{
				ButtonsWrapper.Dispose();
				ButtonsWrapper = null;
			}

			if (SearchButton != null)
			{
				SearchButton.Dispose();
				SearchButton = null;
			}
		}
	}
}
