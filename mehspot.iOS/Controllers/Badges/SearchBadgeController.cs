using Foundation;
using System;
using UIKit;
using Mehspot.iOS.Extensions;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.iOS.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Services;
using Mehspot.iOS.Controllers.Badges.DataSources.Search;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Search;
using System.Reflection;
using System.Linq;

namespace Mehspot.iOS
{
	public partial class SearchBadgeController : UITableViewController, ISearchFilterController
	{
		volatile bool viewWasInitialized;
		public BadgeSummaryDTO BadgeSummary;
		private ISearchQueryDTO SearchQuery;

		public IViewHelper ViewHelper { get; private set; }
		SearchFilterTableSource source;

		public SearchBadgeController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			this.InstantiateQueryDTO(BadgeSummary.BadgeId, BadgeSummary.BadgeName);
			this.TableView.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
			this.TableView.TableFooterView.Hidden = true;
			var badgeService = new BadgeService(MehspotAppContext.Instance.DataStorage);
			this.ViewHelper = new ViewHelper(this.View);
			source = new SearchFilterTableSource(badgeService, BadgeSummary.BadgeId);
			this.TableView.Source = source;
			this.NavBar.Title = this.GetTitle();
		}

		void InstantiateQueryDTO(int badgeId, string badgeName)
		{
			var queryDtoType = Assembly.GetAssembly(typeof(SearchFilterDTOBase))
									 .GetTypes()
									 .FirstOrDefault(a => a
			                                         .GetCustomAttribute<SearchFilterDtoAttribute>()?.BadgeName == badgeName);
			SearchQuery = (ISearchQueryDTO)Activator.CreateInstance(queryDtoType);
			SearchQuery.BadgeId = badgeId;
		}

		public override async void ViewWillAppear(bool animated)
		{
			if (viewWasInitialized)
				return;

			ViewHelper.ShowOverlay("Wait...");
			await source.Initialize(this.SearchQuery);
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
				destinationViewController.SearchQuery = this.SearchQuery;
				destinationViewController.BadgeSummary = this.BadgeSummary;
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