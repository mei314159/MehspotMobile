using Android.Animation;
using Android.App;
using Android.Content;
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
	public class BadgesFragment : Android.Support.V4.App.Fragment, IBadgesViewController
	{
		private BadgesModel model;

		public IViewHelper ViewHelper { get; private set; }


		public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate(Resource.Layout.BadgesActivity, container, false);
		}

		public override void OnViewCreated(Android.Views.View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
			this.ViewHelper = new ActivityHelper(this.Activity);
			model = new BadgesModel(new BadgeService(MehspotAppContext.Instance.DataStorage), this);
			model.LoadingStart += Model_LoadingStart;
			model.LoadingEnd += Model_LoadingEnd;

			var refresher = this.Activity.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
			refresher.SetColorSchemeColors(Resource.Color.xam_dark_blue,
														Resource.Color.xam_purple,
														Resource.Color.xam_gray,
														Resource.Color.xam_green);
			refresher.Refresh += async (sender, e) =>
			{
				await model.RefreshAsync(true);
				refresher.Refreshing = false;
			};
		}

		public override async void OnStart()
		{
			base.OnStart();
			if (!model.dataLoaded)
			{
				await model.RefreshAsync(model.Items == null, true);
			}
		}

		void Model_LoadingStart()
		{
			ViewHelper.ShowOverlay("Loading");
		}

		void Model_LoadingEnd()
		{
			ViewHelper.HideOverlay();
		}

		private BadgeSummaryItem CreateItem(BadgeSummaryDTO dto)
		{
			var item = new BadgeSummaryItem(this.Activity, dto);
			item.Tag = dto.BadgeId;
			item.Clicked += Item_Clicked;
			item.SearchButtonClicked += Item_SearchButtonClicked;
			item.RegisterButtonClicked += Item_RegisterButtonClicked;
			return item;
		}

		void Item_RegisterButtonClicked(BadgeSummaryDTO dto)
		{
			var target = new Intent(this.Context, typeof(EditBadgeProfileActivity));
			target.PutExtra("badgeId", dto.BadgeId);
			target.PutExtra("badgeName", dto.BadgeName);
			target.PutExtra("badgeIsRegistered", dto.IsRegistered);
			this.StartActivity(target);
		}

		void Item_SearchButtonClicked(BadgeSummaryDTO dto)
		{
			var target = new Intent(this.Context, typeof(SearchBadgeActivity));
			target.PutExtra("badgeSummary", dto);
			this.StartActivity(target);
		}

		private void Item_Clicked(BadgeSummaryDTO dto, BadgeSummaryItem sender)
		{
			this.model.SelectBadge(dto);

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
			var wrapper = this.Activity.FindViewById<LinearLayout>(Resource.Id.badgesWrapper);
			wrapper.RemoveAllViews();
			foreach (var item in model.Items)
			{
				var bubble = CreateItem(item);
				wrapper.AddView(bubble);
			}
		}

		private ValueAnimator SlideAnimator(View view, int start, int end)
		{
			var animator = ValueAnimator.OfInt(start, end);
			animator.Update += (sender, e) =>
				{
					var value = (int)animator.AnimatedValue;
					ViewGroup.LayoutParams layoutParams = view.LayoutParameters;
					layoutParams.Height = value;
					view.LayoutParameters = layoutParams;
				};
			return animator;
		}
	}
}
