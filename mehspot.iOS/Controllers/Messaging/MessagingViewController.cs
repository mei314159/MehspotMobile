using System;
using UIKit;
using Mehspot.iOS.Wrappers;
using Mehspot.Core.Messaging;
using System.Collections.Generic;
using Mehspot.Core.DTO;
using Foundation;
using Mehspot.Core;
using Mehspot.Core.Models;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Services;
using Mehspot.iOS.Extensions;
using CoreGraphics;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing;
using SDWebImage;

namespace Mehspot.iOS
{
	public partial class MessagingViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate, IMessagingViewController
	{
		private readonly List<MessageDto> messages = new List<MessageDto>();

		private readonly MessagingModel messagingModel;
		private readonly UIRefreshControl refreshControl;
		private const int spacing = 20;

		public MessagingViewController(IntPtr handle) : base(handle)
		{
			refreshControl = new UIRefreshControl();
			messagingModel = new MessagingModel(new MessagesService(MehspotAppContext.Instance.DataStorage), this);
		}

		public string ToUserName { get; set; }
		public string ToUserId { get; set; }
		public string ProfilePicturePath { get; set; }
		public UIViewController ParentController { get; set; }

		public IViewHelper ViewHelper { get; private set; }

		public event Action Appeared;

		public string MessageFieldValue
		{
			get
			{
				return this.textField.Text;
			}
			set
			{
				this.textField.Text = value;
				LayoutTextInput();
			}
		}


		public override async void ViewDidLoad()
		{
			ViewHelper = new ViewHelper(this.messagesList);

			MehspotAppContext.Instance.ReceivedNotification += OnSendNotification;
			View.BringSubviewToFront(messageFieldWrapper);
			messagesList.RegisterNibForCellReuse(MessageCell.Nib, MessageCell.Key);
			this.View.BackgroundColor = UIColor.White;
			refreshControl.ValueChanged += RefreshControl_ValueChanged;
			messagesList.DataSource = this;
			messagesList.Delegate = this;
			messagesList.AddSubview(refreshControl);
			messagesList.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
			messagesList.RowHeight = UITableView.AutomaticDimension;
			messagesList.EstimatedRowHeight = 50;

			textField.Changed += (sender, e) =>
			{
				LayoutTextInput();
			};

			textField.Layer.BorderWidth = 1;
			textField.Layer.BorderColor = UIColor.LightGray.CGColor;
			RegisterForKeyboardNotifications();

			await ReloadAsync();
		}

		void LayoutTextInput()
		{
			var sizeThatFitsTextView = textField.SizeThatFits(new CGSize(textField.Frame.Size.Width, int.MaxValue));
			TextViewHeightConstraint.Constant = sizeThatFitsTextView.Height > 100 ? 100 : sizeThatFitsTextView.Height;
			textField.LayoutIfNeeded();
		}

		[Action("UnwindToMessagingViewController:")]
		public void UnwindToMessagingViewController(UIStoryboardSegue segue)
		{
		}

		partial void CloseButtonTouched(UIBarButtonItem sender)
		{
			if (ParentController is MessageBoardViewController || ParentController is ViewProfileViewController)
			{
				DismissViewController(true, null);
			}
			else
			{
				PerformSegue("UnwindToSearchResults", this);
			}
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "ShowUserProfileSegue")
			{
				var controller = (UserProfileViewController)segue.DestinationViewController;
				controller.ToUserName = this.ToUserName;
				controller.ToUserId = this.ToUserId;
				controller.ParentController = this;
			}

			base.PrepareForSegue(segue, sender);
		}

		public override void ViewWillAppear(bool animated)
		{
			this.Title = ToUserName;
			this.NavBar.TopItem.Title = ToUserName;

			var composeButton = new UIButton(new RectangleF(0, 0, 30, 30));
			composeButton.ContentMode = UIViewContentMode.ScaleAspectFill;
			composeButton.Layer.CornerRadius = composeButton.Frame.Width / 2;
			composeButton.ClipsToBounds = true;
			composeButton.TouchUpInside += (sender, e) =>
			{
				PerformSegue("ShowUserProfileSegue", this);
			};

			if (!string.IsNullOrEmpty(ProfilePicturePath))
			{
				var url = NSUrl.FromString(ProfilePicturePath);
				if (url != null)
				{
					composeButton.SetBackgroundImage(url, UIControlState.Normal);
				}
			}
			else
			{
				composeButton.SetBackgroundImage(UIImage.FromFile("profile_image.png"), UIControlState.Normal);
			}

			UserPic.CustomView = composeButton;
		}

		public void ScrollToEnd()
		{
			messagesList.ScrollToRow(NSIndexPath.FromRowSection(messages.Count - 1, 0), UITableViewScrollPosition.None, true);
		}

		public override void ViewDidAppear(bool animated)
		{
			Appeared?.Invoke();
		}

		async void RefreshControl_ValueChanged(object sender, EventArgs e)
		{
			await this.messagingModel.LoadMessagesAsync();
			refreshControl.EndRefreshing();
		}

		async partial void SendButtonTouched(UIButton sender)
		{
			await messagingModel.SendMessageAsync();
		}

		async void OnSendNotification(MessagingNotificationType notificationType, MessageDto data)
		{
			if (notificationType == MessagingNotificationType.Message && string.Equals(data.FromUserId, ToUserId, StringComparison.InvariantCultureIgnoreCase))
			{
				InvokeOnMainThread(() =>
				{
					AddMessageBubbleToEnd(data);
				});

				await this.messagingModel.MarkMessagesReadAsync();
			}
		}

		public async Task ReloadAsync()
		{
			refreshControl.BeginRefreshing();
			this.messages.Clear();
			this.messagingModel.Page = 1;
			await this.messagingModel.LoadMessagesAsync();
			await this.messagingModel.MarkMessagesReadAsync();
			refreshControl.EndRefreshing();
		}

		public void DisplayMessages(Result<CollectionDto<MessageDto>> messagesResult)
		{
			InvokeOnMainThread(() =>
			{
				messages.AddRange(messagesResult.Data.Data);
				var rows = messagesResult.Data.Data.Select((a, i) => NSIndexPath.FromRowSection(i, 0)).Reverse().ToArray();
				messagesList.InsertRows(rows, UITableViewRowAnimation.None);
				if (messages.Count > 0 && rows.Length > 0)
				{
					messagesList.ScrollToRow(NSIndexPath.FromRowSection(rows.Length - 1, 0), UITableViewScrollPosition.None, true);
				}
			});
		}

		public void ToggleMessagingControls(bool enabled)
		{
			this.sendButton.Enabled = enabled;
		}

		public void AddMessageBubbleToEnd(MessageDto messageDto)
		{
			messages.Insert(0, messageDto);
			var row = NSIndexPath.FromRowSection(messages.Count - 1, 0);
			messagesList.InsertRows(new[] { row }, UITableViewRowAnimation.None);
			ScrollToEnd();
		}

		public nint RowsInSection(UITableView tableView, nint section)
		{
			return messages?.Count ?? 0;
		}

		public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var itemNumber = (messages.Count - 1) - indexPath.Row; // messages order is ascending
			var cell = tableView.DequeueReusableCell(MessageCell.Key, indexPath) as MessageCell ?? MessageCell.Create();

			var item = messages[itemNumber];
			if (item != null)
				cell.Initialize(item);
			return cell;
		}

		protected virtual void RegisterForKeyboardNotifications()
		{
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
		}

		public void OnKeyboardNotification(NSNotification notification)
		{
			if (!IsViewLoaded)
				return;

			//Check if the keyboard is becoming visible
			var visible = notification.Name == UIKeyboard.WillShowNotification;

			//Start an animation, using values from the keyboard
			UIView.BeginAnimations("AnimateForKeyboard");
			UIView.SetAnimationBeginsFromCurrentState(true);
			UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
			UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

			//Pass the notification, calculating keyboard height, etc.
			var keyboardFrame = visible
									? UIKeyboard.FrameEndFromNotification(notification)
									: UIKeyboard.FrameBeginFromNotification(notification);
			OnKeyboardChanged(visible, keyboardFrame);
			//Commit the animation
			UIView.CommitAnimations();
		}

		public virtual void OnKeyboardChanged(bool visible, CGRect keyboardFrame)
		{
			if (View.Superview == null)
				return;

			if (visible)
			{
				MessageWrapperBottomConstraint.Constant = -keyboardFrame.Height;
				var newY = messagesList.ContentOffset.X + keyboardFrame.Height;
				newY = newY > messagesList.ContentSize.Height ? messagesList.ContentSize.Height : newY;
				messagesList.SetContentOffset(new CGPoint(messagesList.ContentOffset.X, newY), true);
			}
			else
			{
				MessageWrapperBottomConstraint.Constant = 0;
			}

			this.View.LayoutIfNeeded();
		}

		public void ScrollingDown()
		{

		}
	}
}
