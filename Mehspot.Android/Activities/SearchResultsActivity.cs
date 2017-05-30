using System.Threading.Tasks;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Views;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using Mehspot.Models.ViewModels;
using System.Collections.Generic;
using System;

namespace Mehspot.AndroidApp
{

	[Activity(Label = "Search Results")]
	public class SearchResultsActivity : Activity, ISearchResultsController
	{
		private SearchResultsModel model;
		private SearchResultsAdapter searchResultsAdapter;
		public BadgeSummaryDTO BadgeSummary => Intent.GetExtra<BadgeSummaryDTO>("badgeSummary");
		public ISearchQueryDTO SearchQuery => Intent.GetExtra<ISearchQueryDTO>("searchQuery");

		public IViewHelper ViewHelper { get; private set; }

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

			ListView.ScrollChange += Handle_ScrollChange;

			searchResultsAdapter = new SearchResultsAdapter(this);
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

		private ListView ListView => this.FindViewById<ListView>(Resource.SearchResults.ListView);
		private SwipeRefreshLayout Refresher => this.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);

		protected override void OnStart()
		{
			base.OnStart();
			Refresher.Refreshing = true;
			RefreshResults();
		}

		public void ReloadData()
		{
			searchResultsAdapter.Items.Clear();
			searchResultsAdapter.Items.AddRange(model.Items);
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

		void Handle_ScrollChange(object sender, View.ScrollChangeEventArgs e)
		{
			var scrollView = (ListView)sender;
			var currentOffset = e.ScrollY;
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
			target.PutExtra("badgeSummary", this.BadgeSummary);
			target.PutExtra("searchResult", dto);
			this.StartActivity(target);
		}

		void Item_MessageButtonClicked(ISearchResultDTO dto)
		{
			var messagingActivity = new Intent(this, typeof(MessagingActivity));
			messagingActivity.PutExtra("toUserId", dto.Details.UserId);
			messagingActivity.PutExtra("toUserName", dto.Details.FirstName);
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

	public class SearchResultsAdapter : BaseAdapter<ISearchResultDTO>
	{
		public event Action<ISearchResultDTO> MessageButtonClicked;
		public event Action<ISearchResultDTO> ViewProfileButtonClicked;
		public event Action<ISearchResultDTO, SearchResultItem> Clicked;

		public readonly List<ISearchResultDTO> Items;
		Activity context;
		public SearchResultsAdapter(Activity context)
		{
			this.context = context;
			this.Items = new List<ISearchResultDTO>();
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override ISearchResultDTO this[int position]
		{
			get { return Items[position]; }
		}
		public override int Count
		{
			get { return Items.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var view = convertView as SearchResultItem; // re-use an existing view, if one is available
			if (view == null) // otherwise create a new one
			{
				view = new SearchResultItem(context);
				view.Clicked += (sender, e) => Clicked?.Invoke(sender, e);
				view.ViewProfileButtonClicked += (arg1) => ViewProfileButtonClicked?.Invoke(arg1);
				view.MessageButtonClicked += (arg1) => MessageButtonClicked?.Invoke(arg1);
			}

			view.Init(Items[position]);

			return view;
		}
	}
}
