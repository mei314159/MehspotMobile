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

namespace mehspot.iOS
{
	public partial class SearchResultsViewController : UITableViewController
	{
		private volatile bool loading;
		private volatile bool viewWasInitialized;
		private ISearchResultDTO SelectedItem;
		public SearchModel SearchModel;


		public SearchResultsViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			this.TableView.RegisterNibForCellReuse(SearchResultsCell.Nib, SearchResultsCell.Key);
			SearchModel.SearchResultTableSource.SendMessageButtonTouched += SendMessageButtonTouched;
			SearchModel.SearchResultTableSource.ViewProfileButtonTouched += ViewProfileButtonTouched;
			SearchModel.SearchResultTableSource.LoadingMoreStarted += LoadingMoreStarted;
			SearchModel.SearchResultTableSource.LoadingMoreEnded += LoadingMoreEnded;
			SearchModel.SearchResultTableSource.RegisterButtonTouched += SearchResultTableSource_RegisterButtonTouched;
			SearchModel.SearchResultTableSource.OnLoadingError += SearchResultTableSource_OnLoadingError;
			this.TableView.Source = SearchModel.SearchResultTableSource;

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
			var title = MehspotResources.ResourceManager.GetString(this.SearchModel.SearchBadge.BadgeName + "_SearchResultsTitle") ??
			((MehspotResources.ResourceManager.GetString(this.SearchModel.SearchBadge.BadgeName) ?? this.SearchModel.SearchBadge.BadgeName) + "s");
			this.NavBar.Title = title;
		}

		internal void RegqiredBadgeWasRegistered()
		{
			this.viewWasInitialized = false;
			if (SearchModel.SearchBadge.RequiredBadgeId.HasValue)
				SearchModel.SearchBadge.RequiredBadgeIsRegistered = true;
			else {
				SearchModel.SearchBadge.IsRegistered = true;
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
				controller.SearchModel = SearchModel;
				controller.SearchResultDTO = this.SelectedItem;
			}
			else if (segue.Identifier == "RegisterRequiredBadgeSegue")
			{
				var controller = (EditBadgeProfileController)segue.DestinationViewController;

				if (SearchModel.SearchBadge.RequiredBadgeId.HasValue)
				{
					controller.BadgeId = SearchModel.SearchBadge.RequiredBadgeId.Value;
					controller.BadgeName = SearchModel.SearchBadge.RequiredBadgeName;
					controller.BadgeIsRegistered = SearchModel.SearchBadge.RequiredBadgeIsRegistered;
				}
				else
				{
					controller.BadgeId = SearchModel.SearchBadge.BadgeId;
					controller.BadgeName = SearchModel.SearchBadge.BadgeName;
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
			await SearchModel.SearchResultTableSource.LoadDataAsync(this.TableView, true);
			this.TableView.SetContentOffset(CGPoint.Empty, true);
			this.RefreshControl.EndRefreshing();
			loading = false;
		}
	}
}