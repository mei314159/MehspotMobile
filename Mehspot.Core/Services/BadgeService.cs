using System;
using mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Extensions;
using mehspot.Core;
using Mehspot.Core.DTO.Badges;
using MehSpot.Models.ViewModels;
using System.Collections;
using System.Linq;

namespace Mehspot.Core.Services
{
    public class BadgeService : BaseDataService
    {
        public BadgeService(IApplicationDataStorage applicationDataStorage) : base(applicationDataStorage)
        {
        }

        public async Task<Result<BadgeSummaryDTO[]>> GetBadgesSummaryAsync()
        {
            return await GetAsync<BadgeSummaryDTO[]>("Badges/Get").ConfigureAwait(false);
        }

        public async Task<Result<StaticDataDTO[]>> GetBadgeKeysAsync(int badgeId, string key)
        {
            return await GetAsync<StaticDataDTO[]>("Badges/GetBadgeKeys?badgeId=" + badgeId + "&key=" + key).ConfigureAwait(false);
        }

        public async Task<Result<T>> GetBadgeProfileAsync<T>(int badgeId, string userId) where T : IBadgeProfileDTO
        {
            return await GetAsync<T>($"Badges/Profile?badgeId={badgeId}&userId={userId}").ConfigureAwait(false);
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
            return await PostAsync<object>("Badges/SaveProfile", profile).ConfigureAwait(false);
        }

        public async Task<Result> ToggleBadgeUserDescriptionAsync(BadgeUserDescriptionDTO dto)
        {
            return await PostAsync<object>("Badges/ToggleBadgeUserDescription", dto).ConfigureAwait(false);
        }

        public async Task<Result> ToggleEnabledState(int badgeId, bool isEnabled)
        {
            return await PostAsync<object>($"badges/{badgeId}/toggle", isEnabled).ConfigureAwait(false);
        }

        public async Task<Result<BadgeUserRecommendationDTO>> WriteRecommendationAsync(int badgeId, string userId, string text)
        {
            return await PostAsync<BadgeUserRecommendationDTO>($"badges/{badgeId}/recommendations/{userId}", text).ConfigureAwait(false);
        }

        public async Task<Result> DeleteBadgeAsync(int badgeId)
        {
            return await DeleteAsync<object>($"badges/{badgeId}", null).ConfigureAwait(false);
        }

        public class BadgeNames
        {
            public const string Babysitter = "Babysitter";
            public const string BabysitterEmployer = "BabysitterEmployer";
            public const string Fitness = "Fitness";
            public const string Tennis = "Tennis";
            public const string Golf = "Golf";
            public const string Tutor = "Tutor";
            public const string TutorEmployer = "TutorEmployer";
            public const string PetSitter = "PetSitter";
            public const string PetSitterEmployer = "PetSitterEmployer";
            public const string KidsPlayDate = "KidsPlayDate";
            public const string Friendship = "Friendship";
            public const string OtherJobs = "OtherJobs";
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
