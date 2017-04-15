using System;
using System.Threading.Tasks;
using mehspot.iOS.Controllers.Badges.BadgeProfileDataSource;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using MehSpot.Models.ViewModels;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{
    public class SearchModel
    {
        readonly BadgeService badgeService;

        public string BadgeName { get; set; }

        public SearchModel (BadgeService badgeService, string badgeName, ISearchFilterDTO filter)
        {
            this.badgeService = badgeService;
            this.BadgeName = badgeName;
            this.Filter = filter;
        }

        public ISearchFilterDTO Filter { get; private set; }

        public FilterTableSource SearchFilterTableSource { get; private set; }

        public SearchResultTableSource SearchResultTableSource { get; private set; }

        public async Task<ViewBadgeProfileTableSource> GetViewProfileTableSource (string userId)
        {
            ViewBadgeProfileTableSource result = null;

            switch (BadgeName) {
            case BadgeService.BadgeNames.Babysitter:
                result = new ViewBabysitterTableSource (this.Filter.BadgeId, this.BadgeName, badgeService);
                break;
            case BadgeService.BadgeNames.BabysitterEmployer:
                result = new ViewBabysitterEmployerTableSource (this.Filter.BadgeId, this.BadgeName, badgeService);
                break;
            case BadgeService.BadgeNames.Tennis:
                result = new ViewTennisTableSource (this.Filter.BadgeId, this.BadgeName, badgeService);
                break;
            case BadgeService.BadgeNames.Golf:
                result = new ViewGolfTableSource (this.Filter.BadgeId, this.BadgeName, badgeService);
                break;
            }
            await result.LoadAsync (userId);
            return result;
        }

        public static async Task<SearchModel> GetInstanceAsync (BadgeService badgeService, string badgeName, int badgeId)
        {
            FilterTableSource searchFilterTableSource;
            Type resultType;
            switch (badgeName) {
            case BadgeService.BadgeNames.Babysitter:
                searchFilterTableSource = new BabysitterFilterTableSource (badgeService, badgeId);
                resultType = typeof (BabysitterSearchResultDTO []);
                break;
            case BadgeService.BadgeNames.BabysitterEmployer:
                searchFilterTableSource = new BabysitterEmployerFilterTableSource (badgeService, badgeId);
                resultType = typeof (BabysitterEmployerSearchResultDTO []);
                break;
            case BadgeService.BadgeNames.Tennis:
                searchFilterTableSource = new TennisFilterTableSource (badgeService, badgeId);
                resultType = typeof (TennisSearchResultDTO []);
                break;
            case BadgeService.BadgeNames.Golf:
                searchFilterTableSource = new GolfFilterTableSource (badgeService, badgeId);
                resultType = typeof (GolfSearchResultDTO []);
                break;
            default:
                return null;
            }

            await searchFilterTableSource.InitializeAsync ();
            var model = new SearchModel (badgeService, badgeName, searchFilterTableSource.Filter);
            model.SearchFilterTableSource = searchFilterTableSource;
            model.SearchResultTableSource = new SearchResultTableSource (badgeService, searchFilterTableSource.Filter, resultType);
            return model;
        }
    }
}
