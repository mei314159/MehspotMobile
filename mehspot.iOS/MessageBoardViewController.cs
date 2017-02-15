using Foundation;
using System;
using UIKit;
using Mehspot.Core.DTO;
using Mehspot.Core.Messaging;
using mehspot.iOS.Core;
using Mehspot.Core;
using mehspot.iOS.Wrappers;

namespace mehspot.iOS
{
    public partial class MessageBoardViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate
    {
        private readonly MessagesService messagingModel;
        private ViewHelper viewHelper;
        private MessageBoardItemDto [] items;
        private MessageBoardItemDto selectedItem;


        public MessageBoardViewController (IntPtr handle) : base (handle)
        {
            messagingModel = new MessagesService (new ApplicationDataStorage ());

            MehspotAppContext.Instance.ReceivedNotification += OnSendNotification;
        }

        public override void ViewDidLoad ()
        {
            viewHelper = new ViewHelper (this.View);

            MessageBoardTable.RegisterNibForCellReuse (MessageBoardCell.Nib, MessageBoardCell.Key);
            MessageBoardTable.WeakDataSource = this;
            MessageBoardTable.Delegate = this;
        }

        public override async void ViewDidAppear (bool animated)
        {
            viewHelper.ShowOverlay ("Loading Message Board...");
            var messageBoardResult = await messagingModel.GetMessageBoard (this.SearchBar.Text);
            if (messageBoardResult.IsSuccess) {
                this.items = messageBoardResult.Data;
                MessageBoardTable.ReloadData ();
            }
            viewHelper.HideOverlay ();
        }

        //partial void SubmitButtonTouched (UIButton sender)
        //{
        //    GoToMessaging ();
        //}

        //public override void ViewDidLoad ()
        //{
        //    toUserNameField.ShouldReturn += TextFieldShouldReturn;
        //}

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            var controller = (MessagingViewController)segue.DestinationViewController;
            controller.ToUserName = this.selectedItem.WithUser.UserName;
            controller.ToUserId = this.selectedItem.WithUser.Id;
            base.PrepareForSegue (segue, sender);
        }

        void OnSendNotification (MessagingNotificationType notificationType, MessageDto data)
        {
            //if (notificationType == MessagingNotificationType.Message && string.Equals (data.FromUserName, ToUserName, StringComparison.InvariantCultureIgnoreCase)) {
            //    InvokeOnMainThread (() => {
            //        AddMessageBubbleToEnd (data);
            //    });
            //}
        }

        void GoToMessaging ()
        {
            PerformSegue ("GoToMessagingSegue", this);
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
            this.selectedItem = items [indexPath.Row];
            GoToMessaging ();
            tableView.DeselectRow (indexPath, true);
        }

        private void ConfigureCell (MessageBoardCell cell, MessageBoardItemDto item)
        {
            if (!string.IsNullOrEmpty (item.WithUser.ProfilePicturePath))
            {
                cell.ProfilePicture.Image = UIImage.LoadFromData (NSData.FromUrl (NSUrl.FromString (item.WithUser.ProfilePicturePath)));
            }
            cell.ProfilePicture.Layer.CornerRadius = cell.ProfilePicture.Frame.Width / 2;
            cell.UserName.Text = item.WithUser.UserName;
            cell.Message.Text = item.LastMessage;
        }
    }
}