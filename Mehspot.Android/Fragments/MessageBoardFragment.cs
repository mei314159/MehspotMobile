using System;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
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
		public string Filter
		{
			get
			{
				return null;
			}
		}

		public IViewHelper ViewHelper { get; private set; }

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate(Resource.Layout.MessageBoard, container, false);
		}

		public override async void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);

			if (IsPlayServicesAvailable())
			{
				var intent = new Intent(this.Activity, typeof(RegistrationIntentService));
				this.Activity.StartService(intent);
			}


			this.ViewHelper = new ActivityHelper(this.Activity);

			model = new MessageBoardModel(new MessagesService(MehspotAppContext.Instance.DataStorage), this);

			model.LoadingStart += Model_LoadingStart;
			model.LoadingEnd += Model_LoadingEnd;


			var refresher = Activity.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);

			refresher.SetColorSchemeColors(Resource.Color.xam_dark_blue,
														Resource.Color.xam_purple,
														Resource.Color.xam_gray,
														Resource.Color.xam_green);
			refresher.Refresh += async (sender, e) =>
									{
										await model.LoadMessageBoardAsync();
										refresher.Refreshing = false;
									};

			await this.model.LoadMessageBoardAsync();
		}


		void Model_LoadingStart()
		{
			ViewHelper.ShowOverlay("Loading");
		}

		void Model_LoadingEnd()
		{
			ViewHelper.HideOverlay();
		}

		public void DisplayMessageBoard()
		{
			var wrapper = this.Activity.FindViewById<LinearLayout>(Resource.Id.messageBoardWrapper);
			wrapper.RemoveAllViews();
			foreach (var item in model.Items)
			{
				var bubble = CreateMessageBoardItem(item);
				wrapper.AddView(bubble);
			}
		}

		public void UpdateApplicationBadge(int value)
		{

		}

		public void UpdateMessageBoardCell(MessageBoardItemDto dto, int index)
		{
			var wrapper = this.Activity.FindViewById<LinearLayout>(Resource.Id.messageBoardWrapper);
			var item = (MessageBoardItem)wrapper.FindViewWithTag(dto.WithUser.Id);

			item.UnreadMessagesCount.Text = (int.Parse(item.UnreadMessagesCount.Text) + 1).ToString();
			item.Message.Text = dto.LastMessage;
			item.UnreadMessagesCount.Visibility = ViewStates.Visible;
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

			var transaction = this.FragmentManager.BeginTransaction();

			var messagingActivity = new Intent(this.Context, typeof(MessagingActivity));
			messagingActivity.PutExtra("toUserId", toUserId);
			messagingActivity.PutExtra("toUserName", toUserName);

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
	}
}
