﻿
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Widget;
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
	public class MessagingActivity : Activity, IMessagingViewController
	{
		private MessagingModel messagingModel;

		public string ToUserName => Intent.GetStringExtra("toUserName");
		public string ToUserId => Intent.GetStringExtra("toUserId");

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

		public IViewHelper ViewHelper { get; private set; }


		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.MessagingActivity);
			this.ViewHelper = new ActivityHelper(this);
			this.messagingModel = new MessagingModel(new MessagesService(MehspotAppContext.Instance.DataStorage), this);
			MehspotAppContext.Instance.ReceivedNotification += OnSendNotification;

			FindViewById<Button>(Resource.Id.sendMessageButton).Click += SendButtonClicked;
			FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.MessagingActivity.Menu).Title = this.ToUserName;
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
		}

		void OnSendNotification(MessagingNotificationType notificationType, MessageDto data)
		{
			if (notificationType == MessagingNotificationType.Message && string.Equals(data.FromUserId, ToUserId, StringComparison.InvariantCultureIgnoreCase))
			{
				RunOnUiThread(() =>
				{
					AddMessageBubbleToEnd(data);
				});
			}
		}

		async void SendButtonClicked(object sender, EventArgs e)
		{
			await messagingModel.SendMessageAsync();
		}

		public void AddMessageBubbleToEnd(MessageDto messageDto)
		{
			var messagesWrapper = this.FindViewById<LinearLayout>(Resource.Id.messagesWrapper);
			var bubble = CreateMessageBubble(messageDto);
			messagesWrapper.AddView(bubble);
		}

		public void DisplayMessages(Result<CollectionDto<MessageDto>> messagesResult)
		{
			var messagesWrapper = this.FindViewById<LinearLayout>(Resource.Id.messagesWrapper);
			foreach (var messageDto in messagesResult.Data.Data)
			{
				var bubble = CreateMessageBubble(messageDto);
				messagesWrapper.AddView(bubble, 0);
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
			var bubble = new MessageBubble(this, messageDto.Message, isMyMessage);
			return bubble;
		}
	}
}
