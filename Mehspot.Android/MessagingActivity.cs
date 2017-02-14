
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Widget;
using mehspot.Android.Core;
using Mehspot.Android.Resources.layout;
using Mehspot.Android.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.Messaging;
using Mehspot.Core.Models;

namespace Mehspot.Android
{
    [Activity (Label = "Messaging")]
    public class MessagingActivity : Activity, IMessagingViewController
    {
        private MessagingModel messagingModel;

        public string ToUserName => Intent.GetStringExtra ("toUserName");

        public string MessageFieldValue {
            get {
                return FindViewById<TextView> (Resource.Id.messageField).Text;
            }

            set {
                FindViewById<TextView> (Resource.Id.messageField).Text = value;
            }
        }

        public IViewHelper ViewHelper { get; private set; }

        protected override async void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            SetContentView (Resource.Layout.Messaging);

            this.ViewHelper = new ActivityHelper (this);
            this.messagingModel = new MessagingModel (new MessagesService (new ApplicationDataStorage ()), this);
            MehspotAppContext.Instance.ReceivedNotification += OnSendNotification;

            Button button = FindViewById<Button> (Resource.Id.sendMessageButton);
            button.Click += SendButtonClicked;

            var refresher = FindViewById<SwipeRefreshLayout> (Resource.Id.refresher);
            refresher.SetColorScheme (Resource.Color.xam_dark_blue,
                                      Resource.Color.xam_purple,
                                      Resource.Color.xam_gray,
                                      Resource.Color.xam_green);
            refresher.Refresh += async delegate {
                await this.messagingModel.LoadMessagesAsync ();
                refresher.Refreshing = false;
            };

            await this.messagingModel.LoadMessagesAsync ();
        }

        void OnSendNotification (MessagingNotificationType notificationType, MessageDto data)
        {
            if (notificationType == MessagingNotificationType.Message && string.Equals (data.FromUserName, ToUserName, StringComparison.InvariantCultureIgnoreCase)) {
                RunOnUiThread (() => {
                    AddMessageBubbleToEnd (data);
                });
            }
        }

        async void SendButtonClicked (object sender, EventArgs e)
        {
            await messagingModel.SendMessageAsync ();
        }

        public void AddMessageBubbleToEnd (MessageDto messageDto)
        {
            var messagesWrapper = this.FindViewById<LinearLayout> (Resource.Id.messagesWrapper);
            var bubble = CreateMessageBubble (messageDto);
            messagesWrapper.AddView (bubble);
        }

        public void DisplayMessages (Result<CollectionDto<MessageDto>> messagesResult)
        {
            var messagesWrapper = this.FindViewById<LinearLayout> (Resource.Id.messagesWrapper);
            foreach (var messageDto in messagesResult.Data.Data) {
                var bubble = CreateMessageBubble (messageDto);
                messagesWrapper.AddView (bubble, 0);
            }
        }

        public void ToggleMessagingControls (bool enabled)
        {
            var messageField = FindViewById<TextView> (Resource.Id.messageField);
            var sendButton = FindViewById<Button> (Resource.Id.sendMessageButton);
            messageField.Enabled = sendButton.Enabled = enabled;
        }

        private MessageBubble CreateMessageBubble (MessageDto messageDto)
        {
            var isMyMessage = messageDto.FromUserId == MehspotAppContext.Instance.AuthManager.AuthInfo.UserId;
            var bubble = new MessageBubble (this, messageDto.Message, isMyMessage);
            return bubble;
        }
    }
}
