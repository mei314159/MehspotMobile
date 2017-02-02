using Foundation;
using System;
using UIKit;
using mehspot.iOS.Wrappers;
using Mehspot.Core.Messaging;
using mehspot.iOS.Core;
using CoreGraphics;
using System.Linq;
using System.Threading.Tasks;

namespace mehspot.iOS
{
    public partial class MessagingViewController : UIViewController
    {
        private ViewHelper viewHelper;

        public string ToUserName { get; set; }

        public MessagingViewController (IntPtr handle) : base (handle)
        {
            viewHelper = new ViewHelper (this.View);
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
        }

        public override async void ViewDidAppear (bool animated)
        {
            viewHelper.ShowOverlay ("Loading messages...");

            await this.LoadMessagesAsync ();

            viewHelper.HideOverlay ();
        }

        async Task LoadMessagesAsync ()
        {
            var messagesService = new MessagesService (new ApplicationDataStorage ());

            var messagesResult = await messagesService.GetMessages (this.ToUserName, 1);
            nfloat spacing = 20;
            nfloat contentHeight = 0;
            if (messagesResult.IsSuccess) {
                for (var i = 0; i < messagesResult.Messages.Data.Length; i++) {
                    var messageDto = messagesResult.Messages.Data[i];
                    var isMyMessage = messageDto.FromUserId == AppDelegate.AuthManager.AuthInfo.UserId;
                    var bubble = MessageBubble.Create (new CGSize(200, nfloat.MaxValue), messageDto.Message, isMyMessage);
                    var x = isMyMessage ? messagesScrollView.Bounds.Width - bubble.Frame.Width : 0;
                    var y = contentHeight + spacing;
                    bubble.Frame = new CGRect (new CGPoint (x, y), bubble.Frame.Size);
                    contentHeight = y + bubble.Frame.Height;

                    this.messagesScrollView.AddSubview (bubble);
                }

                this.messagesScrollView.ContentSize = new CGSize (messagesScrollView.Bounds.Width, contentHeight > this.messagesScrollView.Frame.Height ? contentHeight : this.messagesScrollView.Frame.Height);
            }
        }
    }
}