﻿using System;
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
    [Activity (Label = "Message Board")]
    public class MessageBoardActivity : Activity, IMessageBoardViewController
    {
        private MessageBoardModel model;
        public string Filter {
            get {
                return null;
            }
        }

        public IViewHelper ViewHelper { get; private set; }

        protected override async void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			SetContentView (Resource.Layout.MessageBoard);

			var tab = ActionBar.NewTab();
			tab.SetText("Messages");
            tab.TabSelected += (sender, args) => this.StartActivity(new Intent(Application.Context, typeof(MessageBoardActivity)));
            ActionBar.AddTab(tab);
            tab = ActionBar.NewTab();
			tab.SetText("Badges");
            tab.TabSelected += (sender, args) => this.StartActivity(new Intent(Application.Context, typeof(BadgesActivity)));
			ActionBar.AddTab(tab);

            if (IsPlayServicesAvailable ()) {
                var intent = new Intent (this, typeof (RegistrationIntentService));

                StartService (intent);
            }

            this.ViewHelper = new ActivityHelper (this);
            model = new MessageBoardModel (new MessagesService (MehspotAppContext.Instance.DataStorage), this);
            model.LoadingStart += Model_LoadingStart;
            model.LoadingEnd += Model_LoadingEnd;

            var refresher = FindViewById<SwipeRefreshLayout> (Resource.Id.refresher);
            refresher.SetColorSchemeColors (Resource.Color.xam_dark_blue,
                                                        Resource.Color.xam_purple,
                                                        Resource.Color.xam_gray,
                                                        Resource.Color.xam_green);
            refresher.Refresh += async (sender, e) => {
                await model.LoadMessageBoardAsync ();
                refresher.Refreshing = false;
            };

            await this.model.LoadMessageBoardAsync ();
        }

        public override void OnBackPressed()
        {
        }

        void Model_LoadingStart ()
        {
            ViewHelper.ShowOverlay ("Loading");
        }

        void Model_LoadingEnd ()
        {
            ViewHelper.HideOverlay ();
        }

        public void DisplayMessageBoard ()
        {
            var wrapper = this.FindViewById<LinearLayout> (Resource.Id.messageBoardWrapper);
            wrapper.RemoveAllViews();
            foreach (var item in model.Items) {
                var bubble = CreateMessageBoardItem (item);
                wrapper.AddView (bubble);
            }
        }

        public void UpdateApplicationBadge (int value)
        {

        }

        public void UpdateMessageBoardCell (MessageBoardItemDto dto, int index)
        {
            var wrapper = this.FindViewById<LinearLayout> (Resource.Id.messageBoardWrapper);
            var item = (MessageBoardItem)wrapper.FindViewWithTag (dto.WithUser.Id);

            item.UnreadMessagesCount.Text = (int.Parse (item.UnreadMessagesCount.Text) + 1).ToString ();
            item.Message.Text = dto.LastMessage;
            item.UnreadMessagesCount.Visibility = ViewStates.Visible;
        }


        private MessageBoardItem CreateMessageBoardItem (MessageBoardItemDto dto)
        {
            var item = new MessageBoardItem (this, dto);
            item.Tag = dto.WithUser.Id;
            item.Clicked += Item_Clicked;
            return item;
        }

        private void Item_Clicked(MessageBoardItemDto dto)
        {
            var toUserId = dto.WithUser.Id;
			var toUserName = dto.WithUser.UserName;
			var messagingActivity = new Intent(Application.Context, typeof(MessagingActivity));
			messagingActivity.PutExtra("toUserId", toUserId);
			messagingActivity.PutExtra("toUserName", toUserName);
			this.StartActivity(messagingActivity);
        }

        public bool IsPlayServicesAvailable ()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable (this);
            if (resultCode != ConnectionResult.Success) {
                if (GoogleApiAvailability.Instance.IsUserResolvableError (resultCode))
                    Console.WriteLine (GoogleApiAvailability.Instance.GetErrorString (resultCode));
                else {
                    Console.WriteLine ("Sorry, this device is not supported");
                    Finish ();
                }
                return false;
            } else {
                Console.WriteLine ("Google Play Services is available.");
                return true;
            }
        }
    }
}
