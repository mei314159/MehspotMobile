using System.Threading.Tasks;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{
    public class SearchModel
    {
        private readonly string badgeName;
        private readonly int badgeId;

        public SearchModel (string badgeName, int badgeId)
        {
            this.badgeName = badgeName;
            this.badgeId = badgeId;
        }

        public ISearchFilterDTO Filter { get; private set; }
        public SearchTableDataSource TableSource { get; private set; }

        public static async Task<SearchModel> GetInstanceAsync (BadgeService badgeService, string badgeName, int badgeId)
        {
            var model = new SearchModel (badgeName, badgeId);
            switch (model.badgeName) {
            case BadgeService.BadgeNames.Babysitter:
                var filter = new SearchBabysitterDTO ();
                model.Filter = filter;
                var tableSource = new SearchBabysitterTableSource (badgeService, model.badgeId, filter);
                await tableSource.InitializeAsync ();
                model.TableSource = tableSource;
                break;
            default:
                break;
            }

            return model;
        }
    }
}
