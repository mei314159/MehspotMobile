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

        public async Task<Result<BadgeSummaryDTO []>> GetBadgesSummaryAsync ()
        {
            return await GetAsync<BadgeSummaryDTO []> ("Badges/Get").ConfigureAwait (false);
        }

        public async Task<Result<StaticDataDTO []>> GetAgeRangesAsync (int badgeId)
        {
            return await GetAsync<StaticDataDTO []> ("Badges/GetAgeRanges?badgeId=" + badgeId).ConfigureAwait (false);
        }

        public async Task<Result<T>> GetBadgeProfileAsync<T> (int badgeId, string userId) where T: IBadgeProfileDTO
        {
            return await GetAsync<T> ($"Badges/Profile?badgeId={badgeId}&userId={userId}").ConfigureAwait (false);
        }

        public async Task<Result<BadgeProfileDTO<EditBadgeProfileDTO>>> GetMyBadgeProfileAsync(int badgeId)
        {
            return await GetAsync<BadgeProfileDTO<EditBadgeProfileDTO>>($"Badges/EditProfile?badgeId={badgeId}&userId={this.ApplicationDataStorage.AuthInfo.UserId}").ConfigureAwait(false);
        }

        public async Task<Result<TResult []>> Search<TResult> (ISearchFilterDTO filter, int skip, int take)
        {
            return await GetAsync<TResult []> ($"Badges/SearchForApp?&skip={skip}&take={take}&" + filter.GetQueryString ()).ConfigureAwait (false);
        }

        public async Task<Result> ToggleBadgeEmploymentHistoryAsync (string userId, int badgeId, bool delete)
        {
            return await PostAsync<object> ($"Badges/ToggleBadgeEmploymentHistory", new { EmployeeId = userId, Delete = delete, BadgeId = badgeId }).ConfigureAwait (false);
        }

        public async Task<Result> SaveBadgeProfileAsync(BadgeProfileDTO<EditBadgeProfileDTO> profile)
        {
            return await PostAsync<object>($"Badges/SaveProfile", profile).ConfigureAwait(false);
        }

        public async Task<Result> ToggleBadgeUserDescriptionAsync (BadgeUserDescriptionDTO dto)
        {
            return await PostAsync<object> ($"Badges/ToggleBadgeUserDescription", dto).ConfigureAwait (false);
        }

        public class BadgeNames
        {
            public const string Babysitter = "Babysitter";
            public const string BabysitterEmployer = "BabysitterEmployer";
            public const string Fitness = "Fitness";
        }
    }

    
}
