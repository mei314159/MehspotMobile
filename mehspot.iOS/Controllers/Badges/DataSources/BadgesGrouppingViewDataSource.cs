﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using mehspot.iOS.Views;
using Mehspot.Core;
using Mehspot.Core.DTO;
using Mehspot.Core.Models;
using Mehspot.Core.Services.Badges;
using Mehspot.iOS.Extensions;
using Mehspot.iOS.Views;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.DataSources
{
	public class BadgesGrouppingViewDataSource : GrouppingViewDataSource<BadgeGroup, BadgeInfo, GrouppingViewCell, BadgeItemCell>
	{
		private readonly BadgesModel model;

		public event Action<BadgeItemCell> SearchButtonTouch;
		public event Action<BadgeItemCell> BadgeRegisterButtonTouch;

		public BadgesGrouppingViewDataSource(BadgesModel model)
		{
			this.model = model;
		}

		public override string GroupViewKey => GrouppingViewCell.Key;
		public override UINib GroupViewNib => GrouppingViewCell.Nib;

		public override string TableViewKey => BadgeItemCell.Key;
		public override UINib TableViewNib => BadgeItemCell.Nib;

		public override GrouppingViewCell CreateGroupView()
		{
			return GrouppingViewCell.Create();
		}

		public override BadgeItemCell CreateTableView()
		{
			return BadgeItemCell.Create();
		}

		public override void InflateView(BadgeItemCell cell, BadgeGroup groupKey, BadgeInfo item)
		{
			cell.Initialize(item);
			cell.SearchButtonTouch = SearchButton_TouchUpInside;
			cell.BadgeRegisterButtonTouch = BadgeRegisterButton_TouchUpInside;
		}

		public override void InflateView(GrouppingViewCell cell, BadgeGroup groupKey)
		{
			cell.Initialize(groupKey);
		}

		[Export("tableView:heightForRowAtIndexPath:")]
		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			if (model.IsRowExpanded(indexPath.Row))
			{
				return 152;
			}
			else
			{
				return 70;
			}
		}

		[Export("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			model.SelectRow(indexPath.Row);
			tableView.ReloadRows(new[] { indexPath }, UITableViewRowAnimation.Fade);
			tableView.ScrollToRow(indexPath, UITableViewScrollPosition.None, true);
		}


		[Export("collectionView:didSelectItemAtIndexPath:")]
		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			model.CollapseRows();
			base.ItemSelected(collectionView, indexPath);
		}

		void SearchButton_TouchUpInside(UIButton button)
		{
			var cell = (BadgeItemCell)button.FindSuperviewOfType(this.TableView, typeof(BadgeItemCell));
			var indexPath = this.TableView.IndexPathForCell(cell);
			this.model.SelectRow(indexPath.Row);
			SearchButtonTouch?.Invoke(cell);
		}

		void BadgeRegisterButton_TouchUpInside(UIButton button)
		{
			var cell = (BadgeItemCell)button.FindSuperviewOfType(this.TableView, typeof(BadgeItemCell));
			var indexPath = this.TableView.IndexPathForCell(cell);
			this.model.SelectRow(indexPath.Row);
			BadgeRegisterButtonTouch?.Invoke(cell);
		}

		public async Task ReloadDataAsync()
		{
			await model.RefreshAsync(model.Items == null, true).ConfigureAwait(false);
		}

		public override void RefreshTable()
		{
			this.Groups = model.BadgeHelper.GetGroups();
			if (CurrentKey == default(int))
				CurrentKey = GetCurrentKey();
			base.RefreshTable();
		}

		BadgeGroup GetCurrentKey()
		{
			var preferredBadgeGroup = MehspotAppContext.Instance.DataStorage.PreferredBadgeGroup;
			if (preferredBadgeGroup == null)
			{
				if (Groups[BadgeGroup.Friends].Any(a => a.Badge.IsRegistered))
				{
					preferredBadgeGroup = BadgeGroup.Friends;
				}
				else if (Groups[BadgeGroup.Helpers].Any(a => a.Badge.IsRegistered))
				{
					preferredBadgeGroup = BadgeGroup.Helpers;
				}
				else if (Groups[BadgeGroup.Jobs].Any(a => a.Badge.IsRegistered))
				{
					preferredBadgeGroup = BadgeGroup.Jobs;
				}
				else
				{
					preferredBadgeGroup = BadgeGroup.Friends;
				}

				MehspotAppContext.Instance.DataStorage.PreferredBadgeGroup = preferredBadgeGroup;
			}

			return preferredBadgeGroup.Value;
		}
	}
}
