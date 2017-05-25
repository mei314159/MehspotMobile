using System.Threading.Tasks;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using Mehspot.Models.ViewModels;

namespace Mehspot.AndroidApp
{

	[Activity(Label = "Search Results")]
	public class SearchResultsActivity : Activity, ISearchResultsController
	{
		private SearchResultsModel model;
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

			this.FindViewById<ScrollView>(Resource.SearchResults.ScrollView).ScrollChange += Handle_ScrollChange; ;


			var refresher = this.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
			refresher.SetColorSchemeColors(Resource.Color.xam_dark_blue,
																	Resource.Color.xam_purple,
																	Resource.Color.xam_gray,
																	Resource.Color.xam_green);
			refresher.Refresh += async (sender, e) =>
			{
				await RefreshResultsAsync();
				refresher.Refreshing = false;
			};
		}

		protected override async void OnStart()
		{
			base.OnStart();
			await RefreshResultsAsync();
		}

		public void ReloadData()
		{
			var wrapper = this.FindViewById<LinearLayout>(Resource.SearchResults.ItemsWrapper);
			wrapper.RemoveAllViews();
			foreach (var item in model.Items)
			{
				var bubble = CreateItem(item);
				wrapper.AddView(bubble);
			}
		}

		private async Task RefreshResultsAsync()
		{
			await this.model.LoadDataAsync(true);
		}

		private SearchResultItem CreateItem(BadgeSummaryDTO dto)
		{
			var item = new SearchResultItem(this.Activity, dto);
			item.Tag = dto.BadgeId;
			item.Clicked += Item_Clicked;
			item.SearchButtonClicked += Item_SearchButtonClicked;
			item.RegisterButtonClicked += Item_RegisterButtonClicked;
			return item;
		}


		private void Item_Clicked(ISearchResultDTO dto, SearchResultItem sender)
		{
			this.model.SelectItem(dto);

			var wrapper = sender.FindViewById(Resource.SearchResultItem.InfoWrapper);

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

		void Handle_ScrollChange(object sender, View.ScrollChangeEventArgs e)
		{
			var scrollView = (ScrollView)sender;
			var currentOffset = e.ScrollY;
			var maximumOffset = scrollView.GetChildAt(0).Height - scrollView.Height;
			var deltaOffset = maximumOffset - currentOffset;
			if (currentOffset > 0 && deltaOffset <= 0)
			{
				model.LoadMoreAsync();
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
