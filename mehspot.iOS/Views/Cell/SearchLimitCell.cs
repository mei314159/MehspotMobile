﻿using System;

using Foundation;
using Mehspot.Core;
using Mehspot.Core.Services;
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

		public static SearchLimitCell Create(string requiredBadgeName, string searchBadgeName)
		{
			var cell = (SearchLimitCell)Nib.Instantiate(null, null)[0];
			cell.RegisterButton.Layer.BorderWidth = 1;
			cell.RegisterButton.Layer.BorderColor = cell.RegisterButton.TitleColor(UIControlState.Normal).CGColor;

			string badgeNameLocalized = null;
			if (requiredBadgeName != null)
			{
				badgeNameLocalized = MehspotResources.ResourceManager.GetString(requiredBadgeName);
				badgeNameLocalized = badgeNameLocalized ?? requiredBadgeName;
			}
			else if (searchBadgeName != null)
			{
				badgeNameLocalized = MehspotResources.ResourceManager.GetString(searchBadgeName);
				badgeNameLocalized = badgeNameLocalized ?? searchBadgeName;
			}
			string badgeNamePart = (requiredBadgeName == Constants.BadgeNames.Fitness || searchBadgeName == Constants.BadgeNames.Golf || requiredBadgeName == Constants.BadgeNames.OtherJobs ? "for " : "as ") + badgeNameLocalized;
			cell.Message.Text = string.Format(MehspotResources.SearchLimitMessageTemplate, badgeNamePart);
			return cell;
		}

		partial void RegisterButtonTouched(UIButton sender)
		{
			this.OnRegisterButtonTouched?.Invoke();
		}
	}
}
