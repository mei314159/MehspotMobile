using System;
using System.Collections.Generic;
using System.Linq;
using Android.Animation;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.View;
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
    public class BadgesFragment : Android.Support.V4.App.Fragment, IBadgesViewController
    {
        private TabLayout tabLayout;
        private BadgesModel model;
        private int? currentTab;
        public IViewHelper ViewHelper { get; private set; }


        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.BadgesActivity, container, false);
        }

        public override void OnViewCreated(Android.Views.View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.ViewHelper = new ActivityHelper(this.Activity);

            tabLayout = View.FindViewById<TabLayout>(Resource.Id.sliding_tabs);
            model = new BadgesModel(new BadgeService(MehspotAppContext.Instance.DataStorage), this);
            model.LoadingStart += Model_LoadingStart;
            model.LoadingEnd += Model_LoadingEnd;
        }

        public override void OnStart()
        {
            base.OnStart();
            (this.Activity as MainActivity)?.SelectTab(this.GetType());
            model.RefreshAsync(model.Items == null, true);
        }

        void Model_LoadingStart()
        {
        }

        void Model_LoadingEnd()
        {
        }

        public void DisplayBadges()
        {
            Activity.RunOnUiThread(() =>
            {
                tabLayout.SetTabTextColors(ContextCompat.GetColor(this.Activity, Resource.Color.black), ContextCompat.GetColor(this.Activity, Resource.Color.dark_orange));
                var viewPager = base.View.FindViewById<ViewPager>(Resource.Id.viewpager);
                if (viewPager != null)
                {
                    //Fragment array
                    var groupKeys = Enum.GetValues(typeof(BadgeGroup)).Cast<BadgeGroup>().ToList();
                    var fragments = groupKeys.Select(a => new BadgeGroupFragment(a, model)).ToArray();
                    //Tab title array
                    var titles = CharSequence.ArrayFromStringArray(groupKeys.Select(a => MehspotResources.ResourceManager.GetString("BadgeGroup_" + a.ToString())).ToArray());
                    //viewpager holding fragment array and tab title text
                    viewPager.Adapter = new TabsFragmentPagerAdapter(ChildFragmentManager, fragments, titles);
                    // Give the TabLayout the ViewPager 
                    tabLayout.SetupWithViewPager(viewPager);
                    viewPager.CurrentItem = currentTab ?? ((int)(MehspotAppContext.Instance.DataStorage.PreferredBadgeGroup ?? BadgeGroup.Friends) - 1);
                    viewPager.PageSelected += (sender, e) => currentTab = e.Position;
                }
            });
        }
    }
}
