using System;
using mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Extensions;
using mehspot.Core;
using Mehspot.Core.DTO.Badges;

namespace Mehspot.Core.Services
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

        public async Task<Result<StaticDataDto []>> GetAgeRangesAsync (int badgeId)
        {
            return await GetAsync<StaticDataDto []> ("Badges/GetAgeRanges?badgeId=" + badgeId).ConfigureAwait (false);
        }

        public async Task<Result<BadgeProfileDTO<BabysitterProfileDTO>>> GetBadgeProfileAsync (int badgeId, string userId)
        {
            return await GetAsync<BadgeProfileDTO<BabysitterProfileDTO>> ($"Badges/Profile?badgeId={badgeId}&userId={userId}").ConfigureAwait (false);
        }

        public async Task<Result<BadgeProfileDTO<EditBabysitterProfileDTO>>> GetMyBadgeProfileAsync(int badgeId)
        {
            return await GetAsync<BadgeProfileDTO<EditBabysitterProfileDTO>>($"Badges/Profile?badgeId={badgeId}&userId={this.ApplicationDataStorage.AuthInfo.UserId}&edit=true").ConfigureAwait(false);
        }

        public async Task<Result<TResult []>> Search<TResult> (ISearchFilterDTO filter, int badgeId, int skip, int take)
        {
            return await GetAsync<TResult []> ($"Badges/SearchForApp?badgeId={badgeId}&skip={skip}&take={take}&" + filter.GetQueryString ()).ConfigureAwait (false);
        }

        public async Task<Result> ToggleBadgeEmploymentHistoryAsync (string userId, int badgeId, bool delete)
        {
            return await PostAsync<object> ($"Badges/ToggleBadgeEmploymentHistory", new { EmployeeId = userId, Delete = delete, BadgeId = badgeId }).ConfigureAwait (false);
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
