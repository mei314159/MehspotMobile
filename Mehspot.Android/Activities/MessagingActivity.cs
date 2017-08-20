using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Net;
using Mehspot.AndroidApp.Resources.layout;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.Messaging;
using Mehspot.Core.Models;
using Mehspot.Core.Services;

namespace Mehspot.AndroidApp
{
	[Activity(Label = "Messaging")]
	public class MessagingActivity : Activity, 	IMessagingViewController
	{
		private MessagingModel messagingModel;

		public ScrollView scrollView => this.FindViewById<ScrollView>(Resource.Id.scrollView1);
		public LinearLayout messagesWrapper => this.FindViewById<LinearLayout>(Resource.Id.messagesWrapper);
		public ImageView UserPicture => this.FindViewById<ImageView>(Resource.MessagingActivity.UserImage);
		public TextView UserNameLabel => this.FindViewById<TextView>(Resource.MessagingActivity.UserName);

		public string ToUserName => Intent.GetStringExtra("toUserName");
		public string ToUserId => Intent.GetStringExtra("toUserId");
		public string ProfilePicturePath => Intent.GetStringExtra("toProfilePicturePath");

		public IViewHelper ViewHelper { get; private set; }

		public string MessageFieldValue
		{
			get
			{
				return FindViewById<TextView>(Resource.Id.messageField).Text;
			}

			set
			{
				FindViewById<TextView>(Resource.Id.messageField).Text = value;
			}
		}

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.MessagingActivity);

			this.ViewHelper = new ActivityHelper(this);
			this.messagingModel = new MessagingModel(new MessagesService(MehspotAppContext.Instance.DataStorage), this);
			MehspotAppContext.Instance.ReceivedNotification += OnSendNotification;

			FindViewById<Button>(Resource.Id.sendMessageButton).Click += SendButtonClicked;
			UserNameLabel.Text = this.ToUserName;
			UserPicture.ClipToOutline = true;
			this.SetImage();

			UserPicture.Click += (sender, e) =>
			{
				var userProfileActivity = new Intent(this, typeof(UserProfileViewActivity));
				userProfileActivity.PutExtra("toUserName", this.ToUserName);
				userProfileActivity.PutExtra("toUserId", this.ToUserId);
				this.StartActivity(userProfileActivity);
			};

			var refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
			refresher.SetColorSchemeColors(Resource.Color.xam_dark_blue,
											Resource.Color.xam_purple,
											Resource.Color.xam_gray,
											Resource.Color.xam_green);
			refresher.Refresh += async (sender, e) =>
			{
				await messagingModel.LoadMessagesAsync();
				refresher.Refreshing = false;
			};

			refresher.Refreshing = true;
			await this.messagingModel.LoadMessagesAsync();
			refresher.Refreshing = false;
			this.ScrollToEnd();
		}

		protected override void OnResume()
		{
			base.OnResume();
		}

		private void SetImage()
		{
			if (!string.IsNullOrEmpty(ProfilePicturePath))
			{
				var url = new URL(ProfilePicturePath);

				if (ProfilePicturePath != null)
				{
					var bitmap = this.GetImageBitmapFromUrl(ProfilePicturePath);
					UserPicture.SetImageBitmap(bitmap);
				}
			}
			else
			{
				UserPicture.SetImageResource(Resource.Drawable.profile_image);
			}
		}

		public void ScrollToEnd()
		{
			RunOnUiThread(() =>
			{
				scrollView.Post(new Runnable(new Action(() =>
				{
					scrollView.FullScroll(Android.Views.FocusSearchDirection.Down);
				})));
			});
		}

		async void OnSendNotification(MessagingNotificationType notificationType, MessageDto data)
		{
			if (notificationType == MessagingNotificationType.Message && string.Equals(data.FromUserId, ToUserId, StringComparison.InvariantCultureIgnoreCase))
			{
				RunOnUiThread(() =>
				{
					AddMessageBubbleToEnd(data);
				});

				await this.messagingModel.MarkMessagesReadAsync();
			}
		}

		async void SendButtonClicked(object sender, EventArgs e)
		{
			await messagingModel.SendMessageAsync();
		}

		public void AddMessageBubbleToEnd(MessageDto messageDto)
		{
			var bubble = CreateMessageBubble(messageDto);
			messagesWrapper.AddView(bubble);
			ScrollToEnd();
		}

		public void DisplayMessages(Result<CollectionDto<MessageDto>> messagesResult)
		{
			foreach (var messageDto in messagesResult.Data.Data)
			{
				var bubble = CreateMessageBubble(messageDto);
				RunOnUiThread(() => messagesWrapper.AddView(bubble, 0));
			}
		}

		public void ToggleMessagingControls(bool enabled)
		{
			var messageField = FindViewById<TextView>(Resource.Id.messageField);
			var sendButton = FindViewById<Button>(Resource.Id.sendMessageButton);
			messageField.Enabled = sendButton.Enabled = enabled;
		}

		private MessageBubble CreateMessageBubble(MessageDto messageDto)
		{
			var isMyMessage = messageDto.FromUserId == MehspotAppContext.Instance.AuthManager.AuthInfo.UserId;
			var bubble = new MessageBubble(this, messageDto.Message, messageDto.SentDate, isMyMessage);
			return bubble;
		}
	}
}
