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
using Mehspot.Core.Models;
using Mehspot.Core.Services;

namespace Mehspot.AndroidApp
{
    [Activity (Label = "Badges Activity")]
    public class BadgesActivity : Activity, IBadgesViewController
    {
        private BadgesModel model;

        public IViewHelper ViewHelper { get; private set; }

        protected override async void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			SetContentView (Resource.Layout.BadgesActivity);

			var tab = ActionBar.NewTab();
			tab.SetText("Messages");
            tab.TabSelected += (sender, args) => this.StartActivity(new Intent(Application.Context, typeof(MessageBoardActivity)));
            ActionBar.AddTab(tab);
            tab = ActionBar.NewTab();
			tab.SetText("Badges");
            tab.TabSelected += (sender, args) => this.StartActivity(new Intent(Application.Context, typeof(BadgesActivity)));
			ActionBar.AddTab(tab);

            this.ViewHelper = new ActivityHelper (this);
            model = new BadgesModel (new BadgeService (MehspotAppContext.Instance.DataStorage), this);
            model.LoadingStart += Model_LoadingStart;
            model.LoadingEnd += Model_LoadingEnd;

            var refresher = FindViewById<SwipeRefreshLayout> (Resource.Id.refresher);
            refresher.SetColorSchemeColors (Resource.Color.xam_dark_blue,
                                                        Resource.Color.xam_purple,
                                                        Resource.Color.xam_gray,
                                                        Resource.Color.xam_green);
            refresher.Refresh += async (sender, e) => {
                await model.RefreshAsync ();
                refresher.Refreshing = false;
            };

            await this.model.RefreshAsync ();
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

        private BadgeSummaryItem CreateMessageBoardItem (BadgeSummaryDTO dto)
        {
            var item = new BadgeSummaryItem (this, dto);
            item.Tag = dto.BadgeId;
            item.Clicked += Item_Clicked;
            return item;
        }

        private void Item_Clicked(BadgeSummaryDTO dto)
        {
   //         var toUserId = dto.WithUser.Id;
			//var toUserName = dto.WithUser.UserName;
			//var messagingActivity = new Intent(Application.Context, typeof(MessagingActivity));
			//messagingActivity.PutExtra("toUserId", toUserId);
			//messagingActivity.PutExtra("toUserName", toUserName);
			//this.StartActivity(messagingActivity);
        }

        public void DisplayBadges()
        {

            var wrapper = this.FindViewById<LinearLayout> (Resource.Id.badgesWrapper);
            wrapper.RemoveAllViews();
            foreach (var item in model.Items) {
                var bubble = CreateMessageBoardItem (item);
                wrapper.AddView (bubble);
            }
        }
    }
}
