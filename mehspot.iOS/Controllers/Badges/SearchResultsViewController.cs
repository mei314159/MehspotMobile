using Foundation;
using System;
using UIKit;
using System.Threading.Tasks;
using CoreGraphics;
using MehSpot.Models.ViewModels;
using mehspot.iOS.Views.Cell;
using mehspot.iOS.Controllers.Badges.DataSources.Search;
using Mehspot.Core;
using mehspot.iOS.Wrappers;
using Mehspot.Core.Services;

namespace mehspot.iOS
{
	public partial class SearchResultsViewController : UITableViewController
	{
		private volatile bool loading;
		private volatile bool viewWasInitialized;
		private ISearchResultDTO SelectedItem;
		public SearchContext SearchContext;
		SearchResultTableSource tableSource;

		public SearchResultsViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			this.TableView.RegisterNibForCellReuse(SearchResultsCell.Nib, SearchResultsCell.Key);
			var badgeService = new BadgeService(MehspotAppContext.Instance.DataStorage);

			tableSource = new SearchResultTableSource(badgeService, this.SearchContext.SearchQuery, this.SearchContext.BadgeSummary, this.SearchContext.SearchResultDtoType);
			tableSource.SendMessageButtonTouched += SendMessageButtonTouched;
			tableSource.ViewProfileButtonTouched += ViewProfileButtonTouched;
			tableSource.LoadingMoreStarted += LoadingMoreStarted;
			tableSource.LoadingMoreEnded += LoadingMoreEnded;
			tableSource.RegisterButtonTouched += SearchResultTableSource_RegisterButtonTouched;
			tableSource.OnLoadingError += SearchResultTableSource_OnLoadingError;
			this.TableView.Source = tableSource;

			this.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
			this.TableView.TableFooterView.Hidden = true;
			SetTitle();
		}

		public override async void ViewDidAppear(bool animated)
		{
			if (!viewWasInitialized)
			{
				await RefreshResultsAsync();
				viewWasInitialized = true;
			}
		}

		private void SetTitle()
		{
			var title = MehspotResources.ResourceManager.GetString(this.SearchContext.BadgeSummary.BadgeName + "_SearchResultsTitle") ??
			((MehspotResources.ResourceManager.GetString(this.SearchContext.BadgeSummary.BadgeName) ?? this.SearchContext.BadgeSummary.BadgeName) + "s");
			this.NavBar.Title = title;
		}

		internal void RegqiredBadgeWasRegistered()
		{
			this.viewWasInitialized = false;
			if (SearchContext.BadgeSummary.RequiredBadgeId.HasValue)
				SearchContext.BadgeSummary.RequiredBadgeIsRegistered = true;
			else
			{
				SearchContext.BadgeSummary.IsRegistered = true;
			}
		}

		[Action("UnwindToSearchResultsViewController:")]
		public void UnwindToSearchResultsViewController(UIStoryboardSegue segue)
		{
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "GoToMessagingSegue")
			{
				var controller = (MessagingViewController)segue.DestinationViewController;
				controller.ToUserName = this.SelectedItem.Details.FirstName;
				controller.ToUserId = this.SelectedItem.Details.UserId;
			}
			else if (segue.Identifier == "ViewProfileSegue")
			{
				var controller = (ViewProfileViewController)segue.DestinationViewController;
				controller.SearchContext = SearchContext;
				controller.SearchResultDTO = this.SelectedItem;
			}
			else if (segue.Identifier == "RegisterRequiredBadgeSegue")
			{
				var controller = (EditBadgeProfileController)segue.DestinationViewController;

				if (SearchContext.BadgeSummary.RequiredBadgeId.HasValue)
				{
					controller.BadgeId = SearchContext.BadgeSummary.RequiredBadgeId.Value;
					controller.BadgeName = SearchContext.BadgeSummary.RequiredBadgeName;
					controller.BadgeIsRegistered = SearchContext.BadgeSummary.RequiredBadgeIsRegistered;
				}
				else
				{
					controller.BadgeId = SearchContext.BadgeSummary.BadgeId;
					controller.BadgeName = SearchContext.BadgeSummary.BadgeName;
					controller.BadgeIsRegistered = false;
				}

				controller.RedirectToSearchResults = true;
			}

			base.PrepareForSegue(segue, sender);
		}

		private void SendMessageButtonTouched(UIButton obj, ISearchResultDTO dto)
		{
			this.SelectedItem = dto;
			PerformSegue("GoToMessagingSegue", this);
		}

		private void ViewProfileButtonTouched(UIButton obj, ISearchResultDTO dto)
		{
			this.SelectedItem = dto;
			PerformSegue("ViewProfileSegue", this);
		}

		private void SearchResultTableSource_RegisterButtonTouched(int requiredBadgeId)
		{
			PerformSegue("RegisterRequiredBadgeSegue", this);
		}

		void SearchResultTableSource_OnLoadingError(Mehspot.Core.DTO.Result result)
		{
			new ViewHelper(this.View).ShowAlert("Search Error", "Please check if you set your Zip Code and Subdivision in your profile.");
		}

		private void LoadingMoreStarted()
		{
			this.ActivityIndicator.StartAnimating();
			this.TableView.TableFooterView.Hidden = false;
		}

		private void LoadingMoreEnded()
		{
			this.ActivityIndicator.StopAnimating();
			this.TableView.TableFooterView.Hidden = true;
		}

		private async void RefreshControl_ValueChanged(object sender, EventArgs e)
		{
			await RefreshResultsAsync();
		}

		private async Task RefreshResultsAsync()
		{
			if (loading)
				return;
			loading = true;
			this.RefreshControl.BeginRefreshing();

			this.TableView.SetContentOffset(new CGPoint(0, -this.RefreshControl.Frame.Size.Height), true);
			await this.tableSource.LoadDataAsync(this.TableView, true);
			this.TableView.SetContentOffset(CGPoint.Empty, true);
			this.RefreshControl.EndRefreshing();
			loading = false;
		}
	}
}