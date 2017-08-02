﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Adapters;
using Mehspot.AndroidApp.Resources.layout;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.Models;
using Mehspot.Core.Services;

namespace Mehspot.AndroidApp
{
	public class MessageBoardFragment : Android.Support.V4.App.Fragment, IMessageBoardViewController
	{
		private MessageBoardModel model;
		private List<MessageBoardItem> wrapList = new List<MessageBoardItem>();

		public string Filter
		{
			get
			{
				return SearchBar.Text;
			}
		}

		public IViewHelper ViewHelper { get; private set; }

		public EditText SearchBar => Activity.FindViewById<EditText>(Resource.MessageBoard.SearchBar);
		public Button SearchButton => Activity.FindViewById<Button>(Resource.MessageBoard.SearchButton);
		public SwipeRefreshLayout refresher => Activity.FindViewById<SwipeRefreshLayout>(Resource.MessageBoard.messageRefresher);

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate(Resource.Layout.MessageBoard, container, false);
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);

			if (IsPlayServicesAvailable())
			{
				var intent = new Intent(this.Activity, typeof(RegistrationIntentService));
				this.Activity.StartService(intent);
			}

			SearchBar.BeforeTextChanged += SearchBar_BeforeTextChanged;
			SearchBar.TextChanged += SearchBar_TextChanged;
			SearchButton.Click += SearchButton_Click;
			this.ViewHelper = new ActivityHelper(this.Activity);

			model = new MessageBoardModel(new MessagesService(MehspotAppContext.Instance.DataStorage), this);

			model.LoadingStart += Model_LoadingStart;
			model.LoadingEnd += Model_LoadingEnd;

			refresher.SetColorSchemeColors(Resource.Color.xam_dark_blue,
														Resource.Color.xam_purple,
														Resource.Color.xam_gray,
														Resource.Color.xam_green);
			refresher.Refresh += async (sender, e) =>
			{
				await model.LoadMessageBoardAsync();
				refresher.Refreshing = false;
			};
		}

		public override async void OnStart()
		{
			base.OnStart();
			if (!model.dataLoaded)
			{
				await this.model.LoadMessageBoardAsync(true);
			}
		}

		void Model_LoadingStart()
		{
			refresher.Refreshing = true;
		}

		void Model_LoadingEnd()
		{
			refresher.Refreshing = false;
		}

		public void DisplayMessageBoard()
		{
			var wrapper = this.Activity.FindViewById<LinearLayout>(Resource.Id.messageBoardWrapper);
			wrapper.RemoveAllViews();

			foreach (var element in wrapList)
			{
				element.Dispose();
			}

			wrapList.Clear();

			foreach (var item in model.Items)
			{
				var bubble = CreateMessageBoardItem(item);
				wrapper.AddView(bubble);
				wrapList.Add(bubble);
			}
		}

		public void UpdateApplicationBadge(int value)
		{

		}

		public void UpdateMessageBoardCell(MessageBoardItemDto dto, int index)
		{
			this.Activity.RunOnUiThread(() =>
			{
				var wrapper = this.Activity.FindViewById<LinearLayout>(Resource.Id.messageBoardWrapper);
				var item = (MessageBoardItem)wrapper.FindViewWithTag(dto.WithUser.Id);

				item.UnreadMessagesCount.Text = (int.Parse(item.UnreadMessagesCount.Text) + 1).ToString();
				item.Message.Text = dto.LastMessage;
				item.UnreadMessagesCount.Visibility = ViewStates.Visible;
			});
		}


		private MessageBoardItem CreateMessageBoardItem(MessageBoardItemDto dto)
		{
			var item = new MessageBoardItem(this.Activity, dto);
			item.Tag = dto.WithUser.Id;
			item.Clicked += Item_Clicked;
			return item;
		}

		private void Item_Clicked(MessageBoardItemDto dto)
		{
			var toUserId = dto.WithUser.Id;
			var toUserName = dto.WithUser.UserName;

			var messagingActivity = new Intent(this.Context, typeof(MessagingActivity));
			messagingActivity.PutExtra("toUserId", toUserId);
			messagingActivity.PutExtra("toUserName", toUserName);
			messagingActivity.PutExtra("toProfilePicturePath", dto.WithUser.ProfilePicturePath);
			this.StartActivity(messagingActivity);
		}

		public bool IsPlayServicesAvailable()
		{
			int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this.Activity);
			if (resultCode != ConnectionResult.Success)
			{
				if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
					Console.WriteLine(GoogleApiAvailability.Instance.GetErrorString(resultCode));
				else
				{
					Console.WriteLine("Sorry, this device is not supported");
					this.Activity.Finish();
				}
				return false;
			}
			else
			{
				Console.WriteLine("Google Play Services is available.");
				return true;
			}
		}

		void SearchBar_BeforeTextChanged(object sender, Android.Text.TextChangedEventArgs e)
		{
			SearchButton.Visibility = ViewStates.Visible;
		}

		void SearchBar_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
		{
			if (e.Text.Count() == 0)
			{
				SearchButton.Text = "Cancel";
			}
			else
			{
				SearchButton.Text = "Search";
			}
		}

		void SearchButton_Click(object sender, EventArgs e)
		{
			SearchButton.Visibility = ViewStates.Gone;
			model.LoadMessageBoardAsync();
		}

	}
}
