using System;
using Mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Extensions;
using Mehspot.Core;
using Mehspot.Core.DTO.Badges;
using Mehspot.Models.ViewModels;
using System.Collections;
using System.Linq;

namespace Mehspot.Core.Services
{
    public class BadgeService : BaseDataService
    {
        public const string BadgeSummaryCacheKey = "BadgeSummaryCacheKey";
        public BadgeService(IApplicationDataStorage applicationDataStorage) : base(applicationDataStorage)
        {
        }

        public BadgeSummaryDTO[] CachedBadgeSummary
        {
            get
            {
                return ApplicationDataStorage.Get<BadgeSummaryDTO[]>(BadgeSummaryCacheKey);
            }
            set
            {
                ApplicationDataStorage.Set(BadgeSummaryCacheKey, value);
            }
        }

        public async Task<Result<BadgeSummaryDTO[]>> GetBadgesSummaryAsync()
        {
            var result = await GetAsync<BadgeSummaryDTO[]>("Badges/Get").ConfigureAwait(false);

            if (result.IsSuccess)
            {
                ApplicationDataStorage.Set(BadgeSummaryCacheKey, result.Data);
            }

            return result;
        }

        public async Task<Result<BadgeSummaryBaseDTO[]>> GetBadgesSummaryBaseAsync(string userId)
        {
            var result = await GetAsync<BadgeSummaryBaseDTO[]>($"Badges/Summary?userId={userId}").ConfigureAwait(false);

            if (result.IsSuccess)
            {
                ApplicationDataStorage.Set(BadgeSummaryCacheKey, result.Data);
            }

            return result;
        }

        public async Task<Result<StaticDataDTO[]>> GetBadgeKeysAsync(int badgeId, string key)
        {
            return await GetAsync<StaticDataDTO[]>("Badges/GetBadgeKeys?badgeId=" + badgeId + "&key=" + key).ConfigureAwait(false);
        }

        public async Task<Result<IBadgeProfileDTO>> GetBadgeProfileAsync(int badgeId, string userId, Type resultType)
        {
            var result = await GetAsync($"Badges/Profile?badgeId={badgeId}&userId={userId}", resultType).ConfigureAwait(false);

            var dto = new Result<IBadgeProfileDTO>
            {
                ErrorMessage = result.ErrorMessage,
                IsSuccess = result.IsSuccess,
                ModelState = result.ModelState
            };

            if (result.IsSuccess)
            {
                dto.Data = result.Data as IBadgeProfileDTO;
            }

            return dto;
        }

        public async Task<Result<BadgeRecommendationDTO>> GetBadgeRecommendationsAsync(int badgeId, string userId)
        {
            return await GetAsync<BadgeRecommendationDTO>($"Badges/Recommendations?badgeId={badgeId}&userId={userId}").ConfigureAwait(false);
        }

        public async Task<Result<BadgeProfileDTO<EditBadgeProfileDTO>>> GetMyBadgeProfileAsync(int badgeId)
        {
            return await GetAsync<BadgeProfileDTO<EditBadgeProfileDTO>>($"Badges/EditProfile?badgeId={badgeId}&userId={this.ApplicationDataStorage.AuthInfo.UserId}").ConfigureAwait(false);
        }

        public async Task<Result<ISearchResultDTO[]>> Search(ISearchQueryDTO filter, int skip, int take, Type resultType)
        {
            var result = await GetAsync($"Badges/SearchForApp?skip={skip}&take={take}&" + filter.GetQueryString(), resultType).ConfigureAwait(false);

            var dto = new Result<ISearchResultDTO[]>
            {
                ErrorMessage = result.ErrorMessage,
                IsSuccess = result.IsSuccess,
                ModelState = result.ModelState
            };
            if (result.IsSuccess)
            {
                var data = result.Data as IEnumerable;
                if (data != null)
                {
                    dto.Data = data.Cast<ISearchResultDTO>().ToArray();
                }
            }

            return dto;
        }

        public async Task<Result> ToggleBadgeEmploymentHistoryAsync(string userId, int badgeId, bool delete)
        {
            return await PostAsync<object>("Badges/ToggleBadgeEmploymentHistory", new { EmployeeId = userId, Delete = delete, BadgeId = badgeId }).ConfigureAwait(false);
        }

        public async Task<Result> SaveBadgeProfileAsync(BadgeProfileDTO<EditBadgeProfileDTO> profile)
        {
            var result = await PostAsync<object>("Badges/SaveProfile", profile).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                UpdateCachedBadgeSummary(profile.BadgeId, true);
            }
            return result;
        }

        public async Task<Result> ToggleBadgeUserDescriptionAsync(BadgeUserDescriptionDTO dto)
        {
            return await PostAsync<object>("Badges/ToggleBadgeUserDescription", dto).ConfigureAwait(false);
        }

        public async Task<Result> ToggleEnabledState(int badgeId, bool isEnabled)
        {
            var result = await PostAsync<object>($"badges/{badgeId}/toggle", isEnabled).ConfigureAwait(false);
            return result;
        }

        public async Task<Result<BadgeUserRecommendationDTO>> WriteRecommendationAsync(int badgeId, string userId, string text)
        {
            return await PostAsync<BadgeUserRecommendationDTO>($"badges/{badgeId}/recommendations/{userId}", text).ConfigureAwait(false);
        }

        public async Task<Result> DeleteBadgeAsync(int badgeId)
        {
            var result = await DeleteAsync<object>($"badges/{badgeId}", null).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                UpdateCachedBadgeSummary(badgeId, false);
            }
            return result;
        }

        private void UpdateCachedBadgeSummary(int badgeId, bool isRegistered)
        {
            var items = CachedBadgeSummary;
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item.BadgeId == badgeId)
                    {
                        item.IsRegistered = isRegistered;
                    }

                    if (item.RequiredBadgeId == badgeId)
                    {
                        item.RequiredBadgeIsRegistered = isRegistered;
                    }
                }

                CachedBadgeSummary = items;
            }
        }

        public class BadgeKeys
        {
            public const string AgeRange = "AgeRange";
            public const string Gender = "Gender";
            public const string SkillLevel = "SkillLevel";
            public const string TennisAgeRange = "TennisAgeRange";
            public const string GolfAgeGroup = "GolfAgeGroup";
            public const string TutorCanTravel = "TutorCanTravel";
            public const string TutorGrade = "TutorGrade";
            public const string TutorSubject = "TutorSubject";
            public const string TutorEmployerSubject = "TutorEmployerSubject";
            public const string TutorEmployerGrade = "TutorEmployerGrade";
            public const string FitnessAgeRange = "FitnessAgeRange";
            public const string FitnessType = "FitnessType";
            public const string PetSitterPetType = "PetSitterPetType";
            public const string PetSitterEmployerPetType = "PetSitterEmployerPetType";
            public const string HobbyAgeRange = "HobbyAgeRange";
            public const string HobbyType = "HobbyType";
            public const string OtherJobsType = "OtherJobsType";
            public const string OtherJobsAgeRange = "OtherJobsAgeRange";
        }

    }
}
