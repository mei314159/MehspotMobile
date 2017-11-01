using Foundation;
using System;
using UIKit;
using MonoTouch.Dialog;
using Mehspot.Core.DTO.Groups;
using Mehspot.Core;
using Mehspot.iOS.Wrappers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.Services;
using System.Collections.Generic;
using System.Collections;
using mehspot.iOS.Views.Cell;
using System.Linq;
using Mehspot.iOS;
using System.Threading.Tasks;

namespace mehspot.iOS
{
    public class GroupInfoViewController : DialogViewController
    {
        private IViewHelper viewHelper;
        private GroupService groupService;
        private GroupInfoMembersSource DataSource;
        private UIRefreshControl refreshControl;

        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public GroupTypeEnum GroupType { get; set; }
        public GroupUserTypeEnum GroupUserType { get; set; }

        public GroupInfoViewController() : base(UITableViewStyle.Grouped, null, true)
        {
        }

        public override void ViewDidLoad()
        {
            this.TableView.RegisterNibForCellReuse(GroupMemberCell.Nib, GroupMemberCell.Key);
            this.viewHelper = new ViewHelper(this.View);
            this.groupService = new GroupService(MehspotAppContext.Instance.DataStorage);
            this.Root = new RootElement(this.GroupName) {
                new Section ("Group Info") {
                    new StringElement ("Name", this.GroupName),
                    new StringElement ("Description", this.GroupDescription),
                    new StringElement ("Group Type", MehspotResources.ResourceManager.GetString($"GroupTypeEnum_{this.GroupType}")),
                    new StringElement ("User Role", MehspotResources.ResourceManager.GetString($"GroupUserTypeEnum_{this.GroupUserType}")),
                }
            };

            this.Root.Add(new Section("Members"));
            if (GroupUserType != GroupUserTypeEnum.Owner)
            {
                UIButton leaveButton = new UIButton(UIButtonType.System);
                leaveButton.SetTitle("Leave Group", UIControlState.Normal);
                leaveButton.TouchUpInside += LeaveButton_TouchUpInside;
                leaveButton.Frame = new CoreGraphics.CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 44);
                leaveButton.SetTitleColor(UIColor.Red, UIControlState.Normal);
                Root.Add(new Section(string.Empty) {
                    new UIViewElement(string.Empty, leaveButton, false)
                });
            }

            this.refreshControl = new UIRefreshControl();
            this.TableView.AddSubview(refreshControl);
            refreshControl.ValueChanged += RefreshControl_ValueChanged;
            LoadDataAsync();
        }

        void LeaveButton_TouchUpInside(object sender, EventArgs e)
        {
            UIAlertController alert = UIAlertController
                    .Create("Please confirm",
                            $"Do you really want to leave a group \"{GroupName}\"?",
                            UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, async (obj) =>
            {
                viewHelper.ShowOverlay("Wait");
                var result = await groupService.LeaveGroupAsync(this.GroupId).ConfigureAwait(false);
                InvokeOnMainThread(() =>
                {
                    viewHelper.HideOverlay();
                    if (result.IsSuccess)
                    {
                        this.DismissViewController(true, null);
                    }
                    else
                    {
                        viewHelper.ShowAlert("Error", result.ErrorMessage);
                    }
                });
            }));

            this.PresentViewController(alert, true, null);

        }

        public override DialogViewController.Source CreateSizingSource(bool unevenRows)
        {
            this.DataSource = new GroupInfoMembersSource(this, this.GroupUserType);
            DataSource.OnRowSelected += DataSource_OnRowSelected;
            return DataSource;
        }

        async void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            InvokeOnMainThread(() => refreshControl.BeginRefreshing());
            var result = await this.groupService.GetMembersAsync(this.GroupId).ConfigureAwait(false);
            InvokeOnMainThread(() =>
            {
                if (result.IsSuccess)
                    this.DataSource.SetData(result.Data);
                this.TableView.ReloadData();
                refreshControl.EndRefreshing();
            });
        }

        void DataSource_OnRowSelected(GroupPrememberDTO dto)
        {
            var controller = (UserProfileViewController)UIStoryboard.FromName("Contact", null).InstantiateViewController("UserProfileViewController");
            controller.ToUserName = dto.Username;
            controller.ToUserId = dto.UserId;
            controller.ParentController = this;
            this.PresentViewController(controller, true, null);
        }
    }

    public class GroupInfoMembersSource : DialogViewController.Source
    {
        readonly GroupUserTypeEnum groupUserType;
        private readonly List<GroupPrememberDTO> membersList = new List<GroupPrememberDTO>();

        public event Action<GroupPrememberDTO> OnRowSelected;

        public GroupInfoMembersSource(DialogViewController container, GroupUserTypeEnum groupUserType) : base(container)
        {
            this.groupUserType = groupUserType;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == 1)
            {
                return this.membersList.Count;
            }

            return base.RowsInSection(tableview, section);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 1)
            {
                var cell = this.Container.TableView.DequeueReusableCell(GroupMemberCell.Key, indexPath) as GroupMemberCell;
                cell.Member = membersList[indexPath.Row];
                return cell;
            }
            else
            {
                return base.GetCell(tableView, indexPath);
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 1)
            {
                var dto = membersList[indexPath.Row];
                this.OnRowSelected?.Invoke(dto);
                return;
            }

            base.RowSelected(tableView, indexPath);
        }

        public void SetData(GroupPrememberDTO[] data)
        {
            lock (((ICollection)this.membersList).SyncRoot)
            {
                membersList.Clear();
                membersList.AddRange(data.OrderBy(x => x.GroupUserType != GroupUserTypeEnum.Owner).ThenBy(x => x.Username));
            }
        }
    }
}