using Foundation;
using System;
using UIKit;
using Mehspot.Core.DTO;
using Mehspot.Core.Messaging;
using mehspot.iOS.Core;
using Mehspot.Core;
using System.Threading.Tasks;
using SDWebImage;
using System.Linq;
using CoreGraphics;

namespace mehspot.iOS
{
    public partial class MessageBoardViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate
    {
        private volatile bool loading;
        private volatile bool goToMessagesWhenAppear;
        private readonly MessagesService messagingModel;

        private MessageBoardItemDto [] items;
        private string SelectedUserId;
        private string SelectedUserName;


        public MessageBoardViewController (IntPtr handle) : base (handle)
        {
            messagingModel = new MessagesService (new ApplicationDataStorage ());

            MehspotAppContext.Instance.ReceivedNotification += OnSendNotification;
        }

        public override void ViewDidLoad ()
        {
            MessageBoardTable.RegisterNibForCellReuse (MessageBoardCell.Nib, MessageBoardCell.Key);
            MessageBoardTable.WeakDataSource = this;
            MessageBoardTable.Delegate = this;
            this.SearchBar.OnEditingStarted += SearchBar_OnEditingStarted;
            this.SearchBar.OnEditingStopped += SearchBar_OnEditingStopped;
            this.SearchBar.CancelButtonClicked += SearchBar_CancelButtonClicked;
            this.SearchBar.SearchButtonClicked += SearchBar_SearchButtonClicked;
            this.MessageBoardTable.RefreshControl = new UIRefreshControl ();
            this.MessageBoardTable.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
        }

        public override async void ViewDidAppear (bool animated)
        {
            await LoadMessageBoardAsync ();
            AppDelegate.CheckPushNotificationsPermissions ();
            if (goToMessagesWhenAppear) {
                goToMessagesWhenAppear = false;
                PerformSegue ("GoToMessagingSegue", this);
            }
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            var controller = (MessagingViewController)segue.DestinationViewController;
            controller.ToUserName = this.SelectedUserName;
            controller.ToUserId = this.SelectedUserId;
            base.PrepareForSegue (segue, sender);
        }

        public nint RowsInSection (UITableView tableView, nint section)
        {
            return items?.Length ?? 0;
        }

        public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = items [indexPath.Row];

            var cell = MessageBoardTable.DequeueReusableCell (MessageBoardCell.Key, indexPath);
            ConfigureCell (cell as MessageBoardCell, item);
            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected (UITableView tableView, NSIndexPath indexPath)
        {
            var dto = items [indexPath.Row].WithUser;
            this.SelectedUserId = dto.Id;
            this.SelectedUserName = dto.UserName;
            PerformSegue ("GoToMessagingSegue", this);
            tableView.DeselectRow (indexPath, true);
        }

        public void ShowMessagesFromUser (string userId, string userName)
        {
            this.SelectedUserId = userId;
            this.SelectedUserName = userName;
            if (this.IsViewLoaded) {
                PerformSegue ("GoToMessagingSegue", this);
            } else {
                goToMessagesWhenAppear = true;
            }
        }

        private async Task LoadMessageBoardAsync ()
        {
            if (loading)
                return;
            loading = true;
            this.MessageBoardTable.RefreshControl.BeginRefreshing ();
            this.MessageBoardTable.SetContentOffset (new CGPoint (0, -this.MessageBoardTable.RefreshControl.Frame.Size.Height), true);
            var messageBoardResult = await messagingModel.GetMessageBoard (this.SearchBar.Text);
            if (messageBoardResult.IsSuccess) {
                this.items = messageBoardResult.Data;
                UpdateApplicationBadge ();
                MessageBoardTable.ReloadData ();
            }

            this.MessageBoardTable.RefreshControl.EndRefreshing ();
            loading = false;
        }

        void UpdateApplicationBadge ()
        {
            UIApplication.SharedApplication.ApplicationIconBadgeNumber =
                 this.items.Length > 0 ? this.items.Sum (a => a.UnreadMessagesCount) : 0;
        }

        private void ConfigureCell (MessageBoardCell cell, MessageBoardItemDto item)
        {
            if (!string.IsNullOrEmpty (item.WithUser.ProfilePicturePath)) {

                var url = NSUrl.FromString (item.WithUser.ProfilePicturePath);
                if (url != null) {
                    cell.ProfilePicture.SetImage (url);
                }
            }
            cell.UserName.Text = item.WithUser.UserName;
            cell.Message.Text = item.LastMessage;
            cell.CountLabel.Hidden = item.UnreadMessagesCount == 0;
            cell.CountLabel.Text = item.UnreadMessagesCount.ToString ();
        }

        private void OnSendNotification (MessagingNotificationType notificationType, MessageDto data)
        {
            if (notificationType == MessagingNotificationType.Message) {
                InvokeOnMainThread (() => {

                    for (int i = 0; i < items.Length; i++) {
                        var item = items [i];
                        if (item.WithUser.Id == data.FromUserId) {
                            var cell = (MessageBoardCell)MessageBoardTable.CellAt (NSIndexPath.FromItemSection (i, 0));
                            cell.CountLabel.Text = (int.Parse (cell.CountLabel.Text) + 1).ToString ();
                            cell.Message.Text = data.Message;
                            cell.CountLabel.Hidden = false;
                            item.UnreadMessagesCount++;
                            UpdateApplicationBadge ();
                            break;
                        }
                    }
                });
            }
        }

        private async void RefreshControl_ValueChanged (object sender, EventArgs e)
        {
            await LoadMessageBoardAsync ();
        }

        private void SearchBar_OnEditingStarted (object sender, EventArgs e)
        {
            SearchBar.SetShowsCancelButton (true, true);
        }

        private void SearchBar_OnEditingStopped (object sender, EventArgs e)
        {
            SearchBar.SetShowsCancelButton (false, true);
            SearchBar.ResignFirstResponder ();
        }

        private async void SearchBar_CancelButtonClicked (object sender, EventArgs e)
        {
            SearchBar.EndEditing (true);
            await LoadMessageBoardAsync ();
        }

        private async void SearchBar_SearchButtonClicked (object sender, EventArgs e)
        {
            SearchBar.EndEditing (true);
            await LoadMessageBoardAsync ();
        }
    }
}