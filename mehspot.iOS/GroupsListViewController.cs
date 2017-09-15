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
            base.ViewDidLoad();
        }

        public override async void ViewDidAppear(bool animated)
        {
            await model.LoadMessageBoardAsync(model.Items == null);
            AppDelegate.CheckPushNotificationsPermissions();
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

        public void DisplayGroups()
        {
            InvokeOnMainThread(() =>
            {
                TableView.ReloadData();
            });
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return model.Items?.Length ?? 0;
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

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            var controller = segue.DestinationViewController as GroupMessagingViewController;
            if (controller != null)
            {
                controller.GroupId = this.SelectedItem.GroupId;
                controller.GroupName = this.SelectedItem.GroupName;
                controller.GroupType = this.SelectedItem.GroupType;
                controller.GroupUserType = this.SelectedItem.GroupUserType;
            }

            base.PrepareForSegue(segue, sender);
        }

        public void UpdateCell(GroupsListItemDTO dto, int index)
        {
            if (dto != null)
            {
                InvokeOnMainThread(() =>
                {
                    var cell = TableView.CellAt(NSIndexPath.FromItemSection(index, 0)) as GroupsListItemCell;
                    if (cell != null)
                    {
                        cell.Item = dto;
                    }
                });
            }
        }

        void Model_LoadingStart()
        {
            this.refreshControl.BeginRefreshing();
            this.TableView.SetContentOffset(new CGPoint(0, -this.refreshControl.Frame.Size.Height), true);
        }

        void Model_LoadingEnd(Result<GroupsListItemDTO[]> result)
        {
            if (result.IsSuccess)
            {
                DisplayGroups();
            }
            InvokeOnMainThread(() =>
            {
                this.TableView.SetContentOffset(CGPoint.Empty, true);
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
            await model.LoadMessageBoardAsync();
        }

        private void ConfigureCell(GroupsListItemCell cell, GroupsListItemDTO item)
        {
            cell.Item = item;
        }
    }
}