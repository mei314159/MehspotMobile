using Foundation;
using System;
using UIKit;
using mehspot.iOS.Wrappers;
using Mehspot.Core.Messaging;
using mehspot.iOS.Core;
using CoreGraphics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client;
using Mehspot.Core;
using Mehspot.Core.DTO;

namespace mehspot.iOS
{
    public partial class MessagingViewController : UIViewController
    {
        private readonly ApplicationDataStorage applicationDataStorage;
        private readonly MessagesService messagesService;
        private readonly UIRefreshControl refreshControl;
        private ViewHelper viewHelper;
        private nfloat spacing = 20;

        public string ToUserName { get; set; }

        public MessagingViewController (IntPtr handle) : base (handle)
        {
            applicationDataStorage = new ApplicationDataStorage ();
            messagesService = new MessagesService (applicationDataStorage);
            refreshControl = new UIRefreshControl ();
            viewHelper = new ViewHelper (View);
        }

        public override async void ViewDidLoad ()
        {
            this.View.BackgroundColor = UIColor.White;
            NavigationItem.Title = ToUserName;
            refreshControl.ValueChanged += RefreshControl_ValueChanged;
            messagesScrollView.AddSubview (refreshControl);

            var hubConnection = new HubConnection (Constants.ApiHost);
            hubConnection.Headers.Add ("Authorization", "Bearer " + applicationDataStorage.AuthInfo.AccessToken);
            var messageNotificationHub = hubConnection.CreateHubProxy ("MessageNotificationHub");

            messageNotificationHub.On<MessagingNotificationType, MessageDto> ("OnSendNotification", OnSendNotification);
            // Start the connection
            await hubConnection.Start ();

            //// Invoke the 'UpdateNick' method on the server
            //await chatHubProxy.Invoke ("UpdateNick", "JohnDoe");
        }

        public override async void ViewDidAppear (bool animated)
        {
            viewHelper.ShowOverlay ("Loading messages...");

            await LoadMessagesAsync ();

            viewHelper.HideOverlay ();
        }

        async void RefreshControl_ValueChanged (object sender, EventArgs e)
        {
            await LoadMessagesAsync ();
            refreshControl.EndRefreshing ();
        }

        private int page = 1;

        async Task LoadMessagesAsync ()
        {
            var messagesResult = await messagesService.GetMessages (ToUserName, page++);

            if (messagesResult.IsSuccess) {
                var existingMessages = messagesScrollView.Subviews.OfType<MessageBubble> ().ToList ();

                nfloat shiftHeight = 0;
                var newMessages = new List<MessageBubble> ();
                foreach (var messageDto in messagesResult.Data.Data.Reverse()) {
                    var bubble = CreateMessageBubble (messageDto, shiftHeight);
                    shiftHeight = bubble.Frame.Y + bubble.Frame.Height;
                    newMessages.Add (bubble);
                }

                foreach (var messageBubble in existingMessages) {
                    messageBubble.Frame = new CGRect (new CGPoint (messageBubble.Frame.X, messageBubble.Frame.Y + shiftHeight), messageBubble.Frame.Size);
                }

                foreach (var messageBubble in newMessages) {
                    messagesScrollView.AddSubview (messageBubble);
                }

                var contentHeight = messagesScrollView.ContentSize.Height + shiftHeight;
                messagesScrollView.ContentSize = new CGSize (messagesScrollView.Bounds.Width, contentHeight > messagesScrollView.Frame.Height ? contentHeight : messagesScrollView.Frame.Height);
                messagesScrollView.ContentOffset = new CGPoint (messagesScrollView.ContentOffset.X, page > 2 ? shiftHeight : 0);
            }
        }

        async partial void SendButtonTouched (UIButton sender)
        {
            if (!string.IsNullOrWhiteSpace (messageField.Text))
            {
                var result = await messagesService.SendMessageAsync (ToUserName, this.messageField.Text);
                if (result.IsSuccess) {
                    AddMessageBubbleToEnd (result.Data);
                    this.messageField.Text = string.Empty;
                }
            }
        }

        void OnSendNotification (MessagingNotificationType notificationType, MessageDto data)
        {
            if (notificationType == MessagingNotificationType.Message) {
                InvokeOnMainThread (() => {
                    AddMessageBubbleToEnd (data);
                });
            }
        }

        void AddMessageBubbleToEnd (MessageDto messageDto)
        {
            var shiftY = messagesScrollView.Subviews.OfType<MessageBubble> ().OrderByDescending (a => a.Frame.Y).Select (a => a.Frame.Y + a.Frame.Height).FirstOrDefault ();
            var bubble = this.CreateMessageBubble (messageDto, shiftY);
            this.messagesScrollView.AddSubview (bubble);
            messagesScrollView.ContentSize = new CGSize (messagesScrollView.Bounds.Width, bubble.Frame.Y + bubble.Frame.Height);
            messagesScrollView.ScrollRectToVisible (new CGRect (messagesScrollView.ContentSize.Width - 1, messagesScrollView.ContentSize.Height - 1, 1, 1), true); //scroll to end programmatically
        }

        private MessageBubble CreateMessageBubble (MessageDto messageDto, nfloat shiftHeight)
        {
            var isMyMessage = messageDto.FromUserId == AppDelegate.AuthManager.AuthInfo.UserId;
            var bubble = MessageBubble.Create (new CGSize (200, nfloat.MaxValue), messageDto.Message, isMyMessage);
            var x = isMyMessage ? messagesScrollView.Bounds.Width - bubble.Frame.Width : 0;
            var y = shiftHeight + spacing;
            bubble.Frame = new CGRect (new CGPoint (x, y), bubble.Frame.Size);

            return bubble;
        }
    }
}