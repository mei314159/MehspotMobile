using Foundation;
using System;
using UIKit;
using Mehspot.iOS.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Services;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.Models;
using CoreGraphics;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Groups;
using System.Linq;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.iOS;

namespace mehspot.iOS
{
	public partial class GroupsListViewController : UITableViewController, IGroupsListViewController
	{
		private volatile bool goToMessagesWhenAppear;

		private GroupsListModel model;
		private UIRefreshControl refreshControl;
		private GroupsListItemDTO SelectedItem;
		private GroupMessageDTO GroupMessage;

		public GroupsListViewController(IntPtr handle) : base(handle)
		{
		}

		public IViewHelper ViewHelper { get; private set; }

		public override void ViewDidLoad()
		{
			this.ViewHelper = new ViewHelper(this.View);
			model = new GroupsListModel(new GroupService(MehspotAppContext.Instance.DataStorage), this);
			model.LoadingStart += Model_LoadingStart;
			model.LoadingEnd += Model_LoadingEnd;

			TableView.RegisterNibForCellReuse(GroupsListItemCell.Nib, GroupsListItemCell.Key);

			this.refreshControl = new UIRefreshControl();
			this.refreshControl.ValueChanged += RefreshControl_ValueChanged;
			this.TableView.AddSubview(refreshControl);
		}

		public override async void ViewDidAppear(bool animated)
		{
			await model.LoadGroupsListAsync(model.Items?.Count == 0);
			AppDelegate.CheckPushNotificationsPermissions();
		}

		public void DisplayGroups()
		{
			InvokeOnMainThread(() => TableView.ReloadData());
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return model.Items.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var item = model.Items[indexPath.Row];

			var cell = tableView.DequeueReusableCell(GroupsListItemCell.Key, indexPath);
			ConfigureCell(cell as GroupsListItemCell, item);
			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.CellAt(indexPath) as GroupsListItemCell;
			this.SelectedItem = cell.Item;
			this.PerformSegue("GoToMessaging", this);
			tableView.DeselectRow(indexPath, true);
		}

		public void ShowMessagesFromGroup(GroupMessageDTO message)
		{
			if (this.IsViewLoaded && this.model.Items != null)
			{
				this.SelectedItem = model.Items.First(a => a.GroupId == message.GroupId);
				this.PerformSegue("GoToMessaging", this);
			}
			else
			{
				goToMessagesWhenAppear = true;
				this.GroupMessage = message;
			}
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
            var controller = (GroupMessagingViewController)((UINavigationController)segue.DestinationViewController).ViewControllers.First();
			if (controller != null)
			{
				controller.GroupId = this.SelectedItem.GroupId;
				controller.GroupName = this.SelectedItem.GroupName;
				controller.GroupDescription = this.SelectedItem.GroupDescription;
                controller.GroupType = this.SelectedItem.GroupType;
				controller.GroupUserType = this.SelectedItem.GroupUserType;
			}

			base.PrepareForSegue(segue, sender);
		}

		public void UpdateList(GroupMessageDTO message)
		{
			model.UpdateList(message);
		}

		void Model_LoadingStart()
		{
			this.InvokeOnMainThread(() =>
			{
				this.refreshControl.BeginRefreshing();
				this.TableView.SetContentOffset(new CGPoint(0, -this.refreshControl.Frame.Size.Height), true);
			});
		}

		void Model_LoadingEnd(Result<GroupsListItemDTO[]> result)
		{
			if (result.IsSuccess)
			{
				DisplayGroups();
			}
			InvokeOnMainThread(() =>
			{
				this.refreshControl.EndRefreshing();

				if (result.IsSuccess && goToMessagesWhenAppear)
				{
					goToMessagesWhenAppear = false;
					ShowMessagesFromGroup(this.GroupMessage);
				}
			});
		}

		private async void RefreshControl_ValueChanged(object sender, EventArgs e)
		{
			await model.LoadGroupsListAsync();
		}

		private void ConfigureCell(GroupsListItemCell cell, GroupsListItemDTO item)
		{
			cell.Item = item;
		}

	}
}