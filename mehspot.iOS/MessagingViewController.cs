using System;
using UIKit;
using mehspot.iOS.Wrappers;
using Mehspot.Core.Messaging;
using mehspot.iOS.Core;
using CoreGraphics;
using System.Linq;
using System.Collections.Generic;
using Mehspot.Core.DTO;
using Foundation;
using Mehspot.Core;
using Mehspot.Core.Models;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.Contracts.ViewControllers;

namespace mehspot.iOS
{
    public partial class MessagingViewController : UIViewController, IMessagingViewController
    {
        private MessagingModel messagingModel;

        private readonly UIRefreshControl refreshControl;
        private const int spacing = 20;

        public MessagingViewController (IntPtr handle) : base (handle)
        {
            refreshControl = new UIRefreshControl ();
            ViewHelper = new ViewHelper (View);
            messagingModel = new MessagingModel (new MessagesService (new ApplicationDataStorage ()), this);
            MehspotAppContext.Instance.ReceivedNotification += OnSendNotification;
        }

        public string ToUserName { get; set; }
        public string ToUserId { get; set; }

        public string MessageFieldValue {
            get {
                return this.messageField.Text;
            }
            set {
                this.messageField.Text = value;
            }
        }

        public IViewHelper ViewHelper { get; private set; }

        protected virtual void RegisterForKeyboardNotifications ()
        {
            NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        public override async void ViewDidLoad ()
        {
            View.BringSubviewToFront (messageFieldWrapper);
            this.View.BackgroundColor = UIColor.White;
            this.Title = ToUserName;

            refreshControl.ValueChanged += RefreshControl_ValueChanged;
            messagesScrollView.AddSubview (refreshControl);
            messagesScrollView.AddGestureRecognizer (new UITapGestureRecognizer (HideKeyboard));
            messageField.ShouldReturn += MessageField_ShouldReturn;
            RegisterForKeyboardNotifications ();
        }

        public void HideKeyboard ()
        {
            messageField.ResignFirstResponder ();
        }

        public override async void ViewDidAppear (bool animated)
        {
            await this.messagingModel.LoadMessagesAsync ();
        }

        async void RefreshControl_ValueChanged (object sender, EventArgs e)
        {
            await this.messagingModel.LoadMessagesAsync ();
            refreshControl.EndRefreshing ();
        }

        async partial void SendButtonTouched (UIButton sender)
        {
            await messagingModel.SendMessageAsync ();
        }

        void OnSendNotification (MessagingNotificationType notificationType, MessageDto data)
        {
            if (notificationType == MessagingNotificationType.Message && string.Equals (data.FromUserId, ToUserId, StringComparison.InvariantCultureIgnoreCase)) {
                InvokeOnMainThread (() => {
                    AddMessageBubbleToEnd (data);
                });
            }
        }

        public void DisplayMessages (Result<CollectionDto<MessageDto>> messagesResult)
        {
            var existingMessages = messagesScrollView.Subviews.OfType<MessageBubble> ().ToList ();

            nfloat shiftHeight = 0;
            var newMessages = new List<MessageBubble> ();
            foreach (var messageDto in messagesResult.Data.Data.Reverse ()) {
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
            messagesScrollView.ContentOffset = new CGPoint (messagesScrollView.ContentOffset.X, this.messagingModel.Page > 2 ? shiftHeight : 0);
        }

        public void ToggleMessagingControls (bool enabled)
        {
            this.sendButton.Enabled = this.messageField.Enabled = enabled;
        }

        public void AddMessageBubbleToEnd (MessageDto messageDto)
        {
            var shiftY = messagesScrollView.Subviews.OfType<MessageBubble> ().OrderByDescending (a => a.Frame.Y).Select (a => a.Frame.Y + a.Frame.Height).FirstOrDefault ();
            var bubble = this.CreateMessageBubble (messageDto, shiftY);
            this.messagesScrollView.AddSubview (bubble);
            messagesScrollView.ContentSize = new CGSize (messagesScrollView.Bounds.Width, bubble.Frame.Y + bubble.Frame.Height);
            messagesScrollView.ScrollRectToVisible (new CGRect (messagesScrollView.ContentSize.Width - 1, messagesScrollView.ContentSize.Height - 1, 1, 1), true); //scroll to end programmatically
        }

        bool MessageField_ShouldReturn (UITextField textField)
        {
            var nextTag = textField.Tag + 1;
            UIResponder nextResponder = this.View.ViewWithTag (nextTag);
            if (nextResponder != null) {
                nextResponder.BecomeFirstResponder ();
            } else {
                // Not found, so remove keyboard.
                textField.ResignFirstResponder ();
                messagingModel.SendMessageAsync ();
            }

            return false; // We do not want UITextField to insert line-breaks.
        }

        public void OnKeyboardNotification (NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            //Check if the keyboard is becoming visible
            var visible = notification.Name == UIKeyboard.WillShowNotification;

            //Start an animation, using values from the keyboard
            UIView.BeginAnimations ("AnimateForKeyboard");
            UIView.SetAnimationBeginsFromCurrentState (true);
            UIView.SetAnimationDuration (UIKeyboard.AnimationDurationFromNotification (notification));
            UIView.SetAnimationCurve ((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification (notification));

            //Pass the notification, calculating keyboard height, etc.
            var keyboardFrame = visible
                                    ? UIKeyboard.FrameEndFromNotification (notification)
                                    : UIKeyboard.FrameBeginFromNotification (notification);
            OnKeyboardChanged (visible, keyboardFrame);
            //Commit the animation
            UIView.CommitAnimations ();
        }

        public virtual void OnKeyboardChanged (bool visible, CGRect keyboardFrame)
        {
            if (View.Superview == null) {
                return;
            }

            if (visible) {
                var relativeLocation = View.Superview.ConvertPointToView (keyboardFrame.Location, View);
                var y = relativeLocation.Y - messageFieldWrapper.Frame.Height;
                messageFieldWrapper.Frame = new CGRect (new CGPoint (messageFieldWrapper.Frame.X, y), messageFieldWrapper.Frame.Size);

            } else {
                var y = messagesScrollView.Frame.Y + messagesScrollView.Frame.Height;
                messageFieldWrapper.Frame = new CGRect (new CGPoint (messageFieldWrapper.Frame.X, y), messageFieldWrapper.Frame.Size);
            }
        }

        private MessageBubble CreateMessageBubble (MessageDto messageDto, nfloat shiftHeight)
        {
            var isMyMessage = messageDto.FromUserId == MehspotAppContext.Instance.AuthManager.AuthInfo.UserId;
            var bubble = MessageBubble.Create (new CGSize (200, nfloat.MaxValue), messageDto.Message, isMyMessage);
            var x = isMyMessage ? messagesScrollView.Bounds.Width - bubble.Frame.Width : 0;
            var y = shiftHeight + spacing;
            bubble.Frame = new CGRect (new CGPoint (x, y), bubble.Frame.Size);

            return bubble;
        }
    }
}