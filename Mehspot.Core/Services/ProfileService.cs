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
            var profile = await this.GetAsync<ProfileDto>("Profile/Get");
            MehspotAppContext.Instance.DataStorage.Profile = profile.Data;
            return profile;
        }

        public async Task<Result<ProfileDto>> UpdateAsync(ProfileDto profile)
        {
            var result = await this.PutAsync<ProfileDto>("profile/update", profile);
            if (result.IsSuccess)
            {
                MehspotAppContext.Instance.DataStorage.Profile = result.Data;
            }

            return result;
        }

        public async Task<Result<string>> UploadProfileImageAsync(Stream profileImage)
        {
            var data = new MultipartFormDataContent();
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
