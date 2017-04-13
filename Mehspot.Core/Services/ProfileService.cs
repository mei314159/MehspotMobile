using System.Net.Http;
using mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using mehspot.Core;

namespace Mehspot.Core.Services
{
    public class ProfileService : BaseDataService
    {
        public ProfileService (IApplicationDataStorage applicationDataStorage) : base (applicationDataStorage)
        {
        }

        public Task<Result<ProfileDto>> GetProfileAsync ()
        {
            return this.GetAsync<ProfileDto> ("Profile/Get");
        }

        public Task<Result<ProfileDto>> UpdateAsync (ProfileDto profile)
        {
            return this.PutAsync<ProfileDto> ("profile/update", profile);
        }

        public async Task<Result> UploadProfileImageAsync (byte [] profileImage)
        {
            var data = new MultipartFormDataContent ();
            var byteArrayContent = new ByteArrayContent (profileImage);
            data.Add (byteArrayContent, "file", "file.jpg");
            return await SendDataAsync<object> ("profile/UploadProfileImage", HttpMethod.Post, data).ConfigureAwait (false);
        }
    }
}
