using Foundation;
using System;
using UIKit;
using Mehspot.iOS.Extensions;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.iOS.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Services;
using Mehspot.Core.DTO;
using Mehspot.iOS.Core.Builders;

namespace Mehspot.iOS
{
	public partial class SearchBadgeController : UITableViewController, ISearchFilterController
	{
		private SearchBadgeModel<UITableViewCell> model;
		public BadgeSummaryDTO BadgeSummary { get; set; }
		public string CustomKey { get; set; }

		public IViewHelper ViewHelper { get; private set; }

		public SearchBadgeController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			this.TableView.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));

			this.ViewHelper = new ViewHelper(this.View);
			var badgeService = new BadgeService(MehspotAppContext.Instance.DataStorage);
			this.model = new SearchBadgeModel<UITableViewCell>(this, badgeService, new IosCellBuilder());
			this.NavBar.Title = this.model.GetTitle();
			this.TableView.EstimatedRowHeight = 44;
			this.TableView.RowHeight = UITableView.AutomaticDimension;
		}

		public override async void ViewWillAppear(bool animated)
		{
			this.TableView.TableFooterView.Hidden = true;
			await this.model.LoadCellsAsync();
			this.TableView.TableFooterView.Hidden = false;
		}

		public void ReloadData()
		{
			this.TableView.ReloadData();
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "SearchResultsSegue")
			{
				var destinationViewController = ((SearchResultsViewController)segue.DestinationViewController);
				destinationViewController.SearchQuery = this.model.SearchQuery;
				destinationViewController.BadgeSummary = this.BadgeSummary;
				this.NavBar.Title = "Filter";
			}

			base.PrepareForSegue(segue, sender);
		}

		partial void SearchButtonTouched(UIButton sender)
		{
			this.PerformSegue("SearchResultsSegue", this);
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var item = model.Cells[indexPath.Row];
			return item;
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return model.Cells.Count;
		}

		public override string TitleForHeader(UITableView tableView, nint section)
		{
			if (section == 0)
			{
				return "Filter";
			}

			return string.Empty;
		}
	}
}