using System.Collections.Generic;
using System.Linq;
using Android.Animation;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Resources.layout;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.Models;
using Mehspot.Core.Services.Badges;

namespace Mehspot.AndroidApp
{
    public class BadgeGroupFragment : Android.Support.V4.App.Fragment
    {

        private BadgeGroup key;
        private List<BadgeSummaryItem> wrapList = new List<BadgeSummaryItem>();
        readonly BadgesModel model;

        public BadgeGroupFragment(BadgeGroup key, BadgesModel model)
        {
            this.model = model;
            this.key = key;
        }

        public IViewHelper ViewHelper { get; private set; }


        public override Android.Views.View OnCreateView(LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.BadgeGroupFragment, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.ViewHelper = new ActivityHelper(this.Activity);
        }

        public override void OnStart()
        {
			DisplayBadges();
            base.OnStart();
        }


        private BadgeSummaryItem CreateItem(BadgeInfo dto)
        {
            var item = new BadgeSummaryItem(this.Activity, dto);
            item.Tag = dto.Badge.BadgeId;
            item.Clicked += Item_Clicked;
            item.SearchButtonClicked += Item_SearchButtonClicked;
            item.RegisterButtonClicked += Item_RegisterButtonClicked;
            return item;
        }

        void Item_RegisterButtonClicked(BadgeInfo dto)
        {
            var target = new Intent(this.Context, typeof(EditBadgeProfileActivity));
            target.PutExtra("badgeId", dto.Badge.BadgeId);
            target.PutExtra("badgeName", dto.Badge.BadgeName);
            target.PutExtra("badgeIsRegistered", dto.Badge.IsRegistered);
            this.StartActivity(target);
        }

        void Item_SearchButtonClicked(BadgeInfo dto)
        {
            var target = new Intent(this.Context, typeof(SearchBadgeActivity));
            target.PutExtra("badgeSummary", model.GetBadgeSummary(dto.SearchBadge));
            target.PutExtra("titleKey", dto.CustomKey);
            this.StartActivity(target);
        }

        private void Item_Clicked(BadgeInfo dto, BadgeSummaryItem sender)
        {
            var wrapper = sender.FindViewById(Resource.BadgeSummary.InfoWrapper);

            if (wrapper.Visibility.Equals(ViewStates.Gone))
            {
                //set Visible
                wrapper.Visibility = ViewStates.Visible;
                int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                wrapper.Measure(widthSpec, heightSpec);

                ValueAnimator mAnimator = SlideAnimator(wrapper, 0, wrapper.MeasuredHeight);
                mAnimator.Start();

            }
            else
            {
                //collapse();
                int finalHeight = wrapper.Height;
                ValueAnimator mAnimator = SlideAnimator(wrapper, finalHeight, 0);
                mAnimator.Start();
                mAnimator.AnimationEnd += (s, e) =>
                {
                    wrapper.Visibility = ViewStates.Gone;
                };
            }
        }

        public void DisplayBadges()
        {
            this.ViewHelper.ShowOverlay("Wait...");
            var wrapper = this.View.FindViewById<LinearLayout>(Resource.Id.badgesWrapper);
            wrapper.RemoveAllViews();
            var badges = model.Groups.Single(a => a.Key == this.key).Value;
            foreach (var item in badges)
            {
                var bubble = CreateItem(item);
                wrapper.AddView(bubble);
                wrapList.Add(bubble);
            }
            this.ViewHelper.HideOverlay();
        }

        private ValueAnimator SlideAnimator(View view, int start, int end)
        {
            var animator = ValueAnimator.OfInt(start, end);
            animator.Update += (sender, e) =>
                {
                    var val = (int)animator.AnimatedValue;
                    ViewGroup.LayoutParams layoutParams = view.LayoutParameters;
                    layoutParams.Height = val;
                    view.LayoutParameters = layoutParams;
                };
            return animator;
        }
    }
}
