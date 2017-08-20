using System;
using System.Threading.Tasks;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.AndroidApp.Adapters;
using Mehspot.Core;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using Mehspot.Models.ViewModels;

namespace Mehspot.AndroidApp
{

	[Activity(Label = "Search Results")]
	public class SearchResultsActivity : Activity, ISearchResultsController, ViewTreeObserver.IOnScrollChangedListener
	{
		private SearchResultsModel model;
		private SearchResultsAdapter searchResultsAdapter;
		private ListView ListView => this.FindViewById<ListView>(Resource.SearchResults.ListView);
		private SwipeRefreshLayout Refresher => this.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);

		public BadgeSummaryDTO BadgeSummary => Intent.GetExtra<BadgeSummaryDTO>("badgeSummary");
		public ISearchQueryDTO SearchQuery => Intent.GetExtra<ISearchQueryDTO>("searchQuery");
		public IViewHelper ViewHelper { get; private set; }

		public string TitleKey => Intent.GetStringExtra("titleKey");

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.SearchResultsActivity);

			this.ViewHelper = new ActivityHelper(this);
			var badgeService = new BadgeService(MehspotAppContext.Instance.DataStorage);
			this.model = new SearchResultsModel(this, badgeService);
			this.Title = this.model.GetTitle();
			model.LoadingMoreStarted += LoadingMoreStarted;
			model.LoadingMoreEnded += LoadingMoreEnded;
			model.OnLoadingError += OnLoadingError;

			ListView.ViewTreeObserver.AddOnScrollChangedListener(this);

			searchResultsAdapter = new SearchResultsAdapter(this, model);
			searchResultsAdapter.MessageButtonClicked += Item_MessageButtonClicked;
			searchResultsAdapter.ViewProfileButtonClicked += Item_ViewProfileButtonClicked;
			searchResultsAdapter.Clicked += Item_Clicked;
			ListView.Adapter = searchResultsAdapter;

			Refresher.SetColorSchemeColors(Resource.Color.xam_dark_blue,
											Resource.Color.xam_purple,
											Resource.Color.xam_gray,
										   	Resource.Color.xam_green);
			Refresher.Refresh += (sender, e) => RefreshResults();
		}

		protected override void OnStart()
		{
			base.OnStart();
			Refresher.Refreshing = true;
			RefreshResults();
		}

		public void ReloadData()
		{
			RunOnUiThread(() =>
			{
				searchResultsAdapter.NotifyDataSetChanged();
				Refresher.Refreshing = false;
			});
		}

		private void RefreshResults()
		{
			Task.Run(async () => await this.model.LoadDataAsync(true));
		}

		void Item_Clicked(ISearchResultDTO dto, Views.SearchResultItem view)
		{
			var wrapper = view.FindViewById(Resource.SearchResultItem.InfoWrapper);
			if (wrapper.Visibility.Equals(ViewStates.Gone) && !this.model.RegisterButtonVisible)
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
				//collapse()
				int finalHeight = wrapper.Height;
				ValueAnimator mAnimator = SlideAnimator(wrapper, finalHeight, 0);
				mAnimator.Start();
				mAnimator.AnimationEnd += (s, ev) =>
				{
					wrapper.Visibility = ViewStates.Gone;
				};
			}

			this.model.SelectItem(dto);
		}

		void ViewTreeObserver.IOnScrollChangedListener.OnScrollChanged()
		{
			var scrollView = ListView;
			var currentOffset = ListView.ScrollY;
			var maximumOffset = (scrollView.GetChildAt(0)?.Height ?? 0) - scrollView.Height;
			var deltaOffset = maximumOffset - currentOffset;
			if (currentOffset > 0 && deltaOffset <= 0)
			{
				Task.Run(async () => await this.model.LoadMoreAsync());
			}
		}

		void OnLoadingError(Mehspot.Core.DTO.Result result)
		{
			this.ViewHelper.ShowAlert("Search Error", "Please check if you set your Zip Code and Subdivision in your profile.");
		}

		private void LoadingMoreStarted()
		{
			//this.ActivityIndicator.StartAnimating();
			//this.TableView.TableFooterView.Hidden = false;
		}

		private void LoadingMoreEnded()
		{
			//this.ActivityIndicator.StopAnimating();
			//this.TableView.TableFooterView.Hidden = true;
		}

		void Item_ViewProfileButtonClicked(ISearchResultDTO dto)
		{
			var target = new Intent(this, typeof(ViewBadgeProfileActivity));
			target.PutExtra("badgeId", this.BadgeSummary.BadgeId);
			target.PutExtra("badgeName", this.BadgeSummary.BadgeName);
			target.PutExtra("userId", dto.Details.UserId);
			target.PutExtra("searchResult", dto);
			this.StartActivity(target);
		}

		void Item_MessageButtonClicked(ISearchResultDTO dto)
		{
			var messagingActivity = new Intent(this, typeof(MessagingActivity));
			messagingActivity.PutExtra("toUserId", dto.Details.UserId);
			messagingActivity.PutExtra("toUserName", dto.Details.FirstName);
			messagingActivity.PutExtra("toProfilePicturePath", dto.Details.ProfilePicturePath);
			this.StartActivity(messagingActivity);
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
