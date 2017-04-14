using System.Threading.Tasks;
using mehspot.iOS.Controllers.Badges.BadgeProfileDataSource;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

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
            this.SearchResultTableSource = new SearchResultTableSource (badgeService, filter);
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
            }

            await result.LoadAsync (userId);
            return result;
        }

        public static async Task<SearchModel> GetInstanceAsync (BadgeService badgeService, string badgeName, int badgeId)
        {
            FilterTableSource searchFilterTableSource;
            switch (badgeName) {
            case BadgeService.BadgeNames.Babysitter:
                searchFilterTableSource = new BabysitterFilterTableSource (badgeService, badgeId);
                break;
            case BadgeService.BadgeNames.BabysitterEmployer:
                searchFilterTableSource = new BabysitterEmployerFilterTableSource (badgeService, badgeId);
                break;
            default:
                return null;
            }

            await searchFilterTableSource.InitializeAsync ();
            var model = new SearchModel (badgeService, badgeName, searchFilterTableSource.Filter);
            model.SearchFilterTableSource = searchFilterTableSource;
            return model;
        }
    }
}
