using Foundation;
using System;
using UIKit;
using System.Threading.Tasks;
using CoreGraphics;
using Mehspot.Models.ViewModels;
using Mehspot.iOS.Views.Cell;
using Mehspot.Core;
using Mehspot.iOS.Wrappers;
using Mehspot.Core.Services;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Contracts.Wrappers;

namespace Mehspot.iOS
{

	public delegate void SendMessageButtonTouched(UIButton obj, ISearchResultDTO item);
	public delegate void ViewProfileButtonTouched(UIButton obj, ISearchResultDTO item);
	public delegate void OnRegisterButtonTouched(int requiredBadgeId);
	public partial class SearchResultsViewController : UITableViewController, ISearchResultsController
	{
		private volatile bool loading;
		private volatile bool viewWasInitialized;
		SearchResultsModel model;

		public IViewHelper ViewHelper { get; set; }
		public BadgeSummaryDTO BadgeSummary { get; set; }
		public ISearchQueryDTO SearchQuery { get; set; }

		public SearchResultsViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			this.TableView.RegisterNibForCellReuse(SearchResultsCell.Nib, SearchResultsCell.Key);
			var badgeService = new BadgeService(MehspotAppContext.Instance.DataStorage);

			this.ViewHelper = new ViewHelper(this.View);
			model = new SearchResultsModel(this, badgeService);
			model.LoadingMoreStarted += LoadingMoreStarted;
			model.LoadingMoreEnded += LoadingMoreEnded;
			model.OnLoadingError += OnLoadingError;

			this.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
			this.TableView.TableFooterView.Hidden = true;
			this.NavBar.Title = model.GetTitle();
		}

		public override async void ViewDidAppear(bool animated)
		{
			if (!viewWasInitialized)
			{
				await RefreshResultsAsync();
				viewWasInitialized = true;
			}
		}

		public void ReloadData()
		{
			TableView.ReloadData();
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			model.SelectRow(indexPath.Row);
			tableView.ReloadRows(new[] { indexPath }, UITableViewRowAnimation.Fade);
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return model.GetRowsCount();
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			if (this.model.RegisterButtonVisible && model.GetRowsCount() == indexPath.Row + 1)
			{
				return SearchLimitCell.Height;
			}

			if (model.IsRowExpanded(indexPath.Row))
			{
				return SearchResultsCell.ExpandedHeight;
			}

			return SearchResultsCell.CollapsedHeight;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = null;
			if (!this.model.RegisterButtonVisible || indexPath.Row + 1 < model.GetRowsCount())
			{
				var item = model.Items[indexPath.Row];
				cell = tableView.DequeueReusableCell(SearchResultsCell.Key, indexPath);
				ConfigureCell(cell as SearchResultsCell, item);
			}
			else if (this.model.RegisterButtonVisible)
			{
				var searchLimitCell = SearchLimitCell.Create(BadgeSummary.RequiredBadgeName, BadgeSummary.BadgeName);
				searchLimitCell.OnRegisterButtonTouched += OnRegisterButtonTouched;
				cell = searchLimitCell;
			}

			return cell;
		}

		[Export("scrollViewDidScroll:")]
		public void Scrolled(UIScrollView scrollView)
		{
			var currentOffset = TableView.ContentOffset.Y;
			var maximumOffset = TableView.ContentSize.Height - TableView.Frame.Size.Height;
			var deltaOffset = maximumOffset - currentOffset;

			if (currentOffset > 0 && deltaOffset <= 0)
			{
				model.LoadMoreAsync();
			}
		}

		private void ConfigureCell(SearchResultsCell cell, ISearchResultDTO item)
		{
			cell.Configure(item);

			cell.SendMessageButtonAction = (obj) => SendMessageButtonTouched(obj, item);
			cell.ViewProfileButtonAction = (obj) => ViewProfileButtonTouched(obj, item);
		}

		internal void ReqiredBadgeWasRegistered()
		{
			this.viewWasInitialized = false;
			if (BadgeSummary.RequiredBadgeId.HasValue)
				BadgeSummary.RequiredBadgeIsRegistered = true;
			else
			{
				BadgeSummary.IsRegistered = true;
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
				controller.ToUserName = this.model.SelectedItem.Details.FirstName;
				controller.ToUserId = this.model.SelectedItem.Details.UserId;
				controller.ProfilePicturePath = this.model.SelectedItem.Details.ProfilePicturePath;
			}
			else if (segue.Identifier == "ViewProfileSegue")
			{
				var controller = (ViewProfileViewController)segue.DestinationViewController;
				controller.BadgeId = this.BadgeSummary.BadgeId;
				controller.BadgeName = this.BadgeSummary.BadgeName;
				controller.UserId = this.model.SelectedItem.Details.UserId;
			}
			else if (segue.Identifier == "RegisterRequiredBadgeSegue")
			{
				var controller = (EditBadgeProfileController)segue.DestinationViewController;

				if (BadgeSummary.RequiredBadgeId.HasValue)
				{
					controller.BadgeId = BadgeSummary.RequiredBadgeId.Value;
					controller.BadgeName = BadgeSummary.RequiredBadgeName;
					controller.BadgeIsRegistered = BadgeSummary.RequiredBadgeIsRegistered;
				}
				else
				{
					controller.BadgeId = BadgeSummary.BadgeId;
					controller.BadgeName = BadgeSummary.BadgeName;
					controller.BadgeIsRegistered = false;
				}

				controller.RedirectToSearchResults = true;
			}

			base.PrepareForSegue(segue, sender);
		}

		private void SendMessageButtonTouched(UIButton obj, ISearchResultDTO dto)
		{
			this.model.SelectItem(dto);
			PerformSegue("GoToMessagingSegue", this);
		}

		private void ViewProfileButtonTouched(UIButton obj, ISearchResultDTO dto)
		{
			this.model.SelectItem(dto);
			PerformSegue("ViewProfileSegue", this);
		}

		void OnRegisterButtonTouched()
		{
			PerformSegue("RegisterRequiredBadgeSegue", this);
		}

		void OnLoadingError(Result result)
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
			await this.model.LoadDataAsync(true);
			this.TableView.SetContentOffset(CGPoint.Empty, true);
			this.RefreshControl.EndRefreshing();
			loading = false;
		}
	}
}