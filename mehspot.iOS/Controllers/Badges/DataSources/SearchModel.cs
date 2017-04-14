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

        public SearchFilterTableSource SearchFilterTableSource { get; private set; }

        public SearchResultTableSource SearchResultTableSource { get; private set; }

        public async Task<ViewBadgeProfileTableSource> GetViewProfileTableSource (string userId)
        {
            var result = await badgeService.GetBadgeProfileAsync (this.Filter.BadgeId, userId);
            if (result.IsSuccess) {
                var profile = result.Data;

                switch (BadgeName) {
                case BadgeService.BadgeNames.Babysitter: {
                        return new ViewBabysitterTableSource (profile, this.Filter.BadgeId, this.BadgeName, badgeService);
                    }
                }
            }

            return null;
        }

        public static async Task<SearchModel> GetInstanceAsync (BadgeService badgeService, string badgeName, int badgeId)
        {
            SearchBabysitterDTO filter;
            SearchFilterTableSource tableSource;
            switch (badgeName) {
            case BadgeService.BadgeNames.Babysitter:
                filter = new SearchBabysitterDTO (badgeId);
                tableSource = new SearchBabysitterFilterTableSource (badgeService, filter);
                await tableSource.InitializeAsync ();

                break;
            default:
                return null;
            }

            var model = new SearchModel (badgeService, badgeName, filter);
            model.SearchFilterTableSource = tableSource;
            return model;
        }
    }
}
