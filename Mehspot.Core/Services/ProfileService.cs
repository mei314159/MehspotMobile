using System.Net.Http;
using Mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using Mehspot.Core;
using System.IO;

namespace Mehspot.Core.Services
{
    public class ProfileService : BaseDataService
    {
        public ProfileService(IApplicationDataStorage applicationDataStorage) : base(applicationDataStorage)
        {
        }

        public ProfileDto Profile => MehspotAppContext.Instance.DataStorage.Profile;

        public async Task<Result<ProfileDto>> LoadProfileAsync()
        {
            var result = await this.GetAsync<ProfileDto>("Profile/Get");
			if (result.IsSuccess)
                MehspotAppContext.Instance.DataStorage.Profile = result.Data;
            return result;
        }

        public async Task<Result<UserProfileSummaryDTO>> LoadProfileSummaryAsync(string userId)
        {
            var result = await this.GetAsync<UserProfileSummaryDTO>($"Profile/Summary?userId={userId}");
            if (result.IsSuccess)
                MehspotAppContext.Instance.DataStorage.UserProfile = result.Data;
            return result;
        }

        public async Task<Result<ProfileDto>> UpdateAsync(ProfileDto profile)
        {
            var result = await this.PutAsync<ProfileDto>("Profile/Update", profile).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                MehspotAppContext.Instance.DataStorage.Profile = result.Data;
            }

            return result;
        }

        public async Task<Result<string>> UploadProfileImageAsync(Stream profileImage)
        {
            var data = new MultipartFormDataContent();
            profileImage.Position = 0;
            var streamContent = new StreamContent(profileImage);
            data.Add(streamContent, "file", "file.jpg");
            var result = await SendDataAsync<string>("profile/UploadProfileImage", HttpMethod.Post, data).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                var profile = MehspotAppContext.Instance.DataStorage.Profile;
                profile.ProfilePicturePath = result.Data;
                MehspotAppContext.Instance.DataStorage.Profile = profile;
            }

            return result;
        }
    }
}
