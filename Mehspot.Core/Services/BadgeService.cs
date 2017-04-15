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

        public async Task<Result<BadgeProfileDTO<EditBadgeProfileDTO>>> GetMyBadgeProfileAsync(int badgeId)
        {
            return await GetAsync<BadgeProfileDTO<EditBadgeProfileDTO>>($"Badges/EditProfile?badgeId={badgeId}&userId={this.ApplicationDataStorage.AuthInfo.UserId}").ConfigureAwait(false);
        }

        public async Task<Result<ISearchResultDTO[]>> Search(ISearchFilterDTO filter, int skip, int take, Type resultType)
        {
            var result = await GetAsync($"Badges/SearchForApp?badgeId={filter.BadgeId}&skip={skip}&take={take}&" + filter.GetQueryString(), resultType).ConfigureAwait(false);

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
            return await PostAsync<object>($"Badges/ToggleBadgeEmploymentHistory", new { EmployeeId = userId, Delete = delete, BadgeId = badgeId }).ConfigureAwait(false);
        }

        public async Task<Result> SaveBadgeProfileAsync(BadgeProfileDTO<EditBadgeProfileDTO> profile)
        {
            return await PostAsync<object>($"Badges/SaveProfile", profile).ConfigureAwait(false);
        }

        public async Task<Result> ToggleBadgeUserDescriptionAsync(BadgeUserDescriptionDTO dto)
        {
            return await PostAsync<object>($"Badges/ToggleBadgeUserDescription", dto).ConfigureAwait(false);
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
            public const string FitnessAgeRange = "FitnessAgeRange";
            public const string FitnessType = "FitnessType";
        }
    }


}
