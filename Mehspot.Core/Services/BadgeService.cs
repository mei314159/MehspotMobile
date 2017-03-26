using System;
using mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Extensions;
using MehSpot.Models.ViewModels;
using MehSpot.Web.ViewModels;
using mehspot.Core;

namespace Mehspot.Core
{
    public class BadgeService : BaseDataService
    {
        public BadgeService (IApplicationDataStorage applicationDataStorage) : base (applicationDataStorage)
        {
        }

        public Action<int, object> OnSendNotification;

        public async Task<Result<BadgeSummaryDTO []>> GetBadgesSummaryAsync ()
        {
            return await GetAsync<BadgeSummaryDTO []> ("Badges/Get").ConfigureAwait (false);
        }

        public async Task<Result<StaticDataDto []>> GetAgeRangesAsync (string badgeName)
        {
            return await GetAsync<StaticDataDto []> ("Badges/GetAgeRanges?badgeName=" + badgeName).ConfigureAwait (false);
        }

        public async Task<Result<BadgeProfileDTO<BabysitterProfileDTO>>> GetBadgeProfileAsync (string badgeName, string userId)
        {
            return await GetAsync<BadgeProfileDTO<BabysitterProfileDTO>> ($"Badges/Profile?badgeName={badgeName}&userId={userId}").ConfigureAwait (false);
        }

        public async Task<Result<TResult []>> Search<TResult> (ISearchFilterDTO filter, string badgeName, int skip, int take)
        {
            return await GetAsync<TResult []> ($"Badges/SearchForApp?badgeName={badgeName}&skip={skip}&take={take}&" + filter.GetQueryString ()).ConfigureAwait (false);
        }

        public async Task<Result> ToggleBadgeEmploymentHistoryAsync (string userId, string badgeName, bool delete)
        {
            return await PostAsync<object> ($"Badges/ToggleBadgeEmploymentHistory", new { EmployeeId = userId, Delete = delete, BadgeName = badgeName }).ConfigureAwait (false);
        }

        public async Task<Result> ToggleBadgeUserDescriptionAsync (BadgeUserDescriptionDTO dto)
        {
            return await PostAsync<object> ($"Badges/ToggleBadgeUserDescription", dto).ConfigureAwait (false);
        }

        public class BadgeNames
        {
            public const string Babysitter = "Babysitter";
        }
    }
}
