using Foundation;
using System;
using UIKit;
using mehspot.iOS.Extensions;
using Mehspot.Core.Contracts.Wrappers;
using mehspot.iOS.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Services;
using mehspot.iOS.Controllers.Badges.DataSources.Search;
using Mehspot.Core.DTO;

namespace mehspot.iOS
{
	public partial class SearchBadgeController : UITableViewController, ISearchFilterController
	{
		volatile bool viewWasInitialized;
		private SearchContext model;
		public BadgeSummaryDTO BadgeSummary;


		public IViewHelper ViewHelper { get; private set; }
		SearchFilterTableSource source;

		public SearchBadgeController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			this.TableView.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
			this.TableView.TableFooterView.Hidden = true;
			var badgeService = new BadgeService(MehspotAppContext.Instance.DataStorage);
			this.ViewHelper = new ViewHelper(this.View);
			this.model = new SearchContext(badgeService, this.BadgeSummary);
			source = new SearchFilterTableSource(badgeService, BadgeSummary.BadgeId);
			this.TableView.Source = source;
			this.NavBar.Title = this.GetTitle();
		}

		public override async void ViewWillAppear(bool animated)
		{
			if (viewWasInitialized)
				return;

			ViewHelper.ShowOverlay("Wait...");
			await source.Initialize(model.SearchQuery);
			this.TableView.ReloadData();

			ViewHelper.HideOverlay();
			viewWasInitialized = true;
			this.TableView.TableFooterView.Hidden = false;
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "SearchResultsSegue")
			{
				var destinationViewController = ((SearchResultsViewController)segue.DestinationViewController);
				destinationViewController.SearchContext = this.model;
				this.NavBar.Title = "Filter";
			}

			base.PrepareForSegue(segue, sender);
		}

		partial void SearchButtonTouched(UIButton sender)
		{
			this.PerformSegue("SearchResultsSegue", this);
		}

		public string GetTitle()
		{
			var badgeName = MehspotResources.ResourceManager.GetString(this.BadgeSummary.BadgeName) ?? this.BadgeSummary.BadgeName;
			var title = "Search for " + badgeName;
			return title;
		}
	}
}