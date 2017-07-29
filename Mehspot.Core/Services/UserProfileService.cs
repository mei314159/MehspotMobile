using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Badges;

namespace Mehspot.Core.Services
{
    public class UserProfileService : BaseDataService
    {
        public UserProfileSummaryDTO Profile => MehspotAppContext.Instance.DataStorage.UserProfile;
        public const string BadgeSummaryCacheKey = "BadgeSummaryCacheKey";

        public UserProfileService(IApplicationDataStorage applicationDataStorage) : base(applicationDataStorage)
        {
        }

        public async Task<Result<UserProfileSummaryDTO>> LoadUserProfileAsync()
        {
            var userProfile = await this.GetAsync<UserProfileSummaryDTO>("Profile/Get"); // shto eto takoe
            MehspotAppContext.Instance.DataStorage.UserProfile = userProfile.Data;
            return userProfile;
        }

        public async Task<Result<string>> UploadUserProfileImageAsync(Stream userProfileImage)
        {
            var data = new MultipartFormDataContent();
            var streamContent = new StreamContent(userProfileImage);
            data.Add(streamContent, "file", "file.jpg");
            var result = await SendDataAsync<string>("profile/UploadProfileImage", HttpMethod.Post, data).ConfigureAwait(false); // Change "profile/UploadProfileImage"

            if (result.IsSuccess)
            {
                var userProfile = MehspotAppContext.Instance.DataStorage.UserProfile;
                userProfile.ProfilePicturePath = result.Data;
                MehspotAppContext.Instance.DataStorage.UserProfile = userProfile;
            }

            return result;
        }

        public async Task<Result<BadgeSummaryBaseDTO[]>> GetBadgesSummaryAsync()
        {
            var result = await GetAsync<BadgeSummaryBaseDTO[]>("Badges/Get").ConfigureAwait(false);

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
    }
}
