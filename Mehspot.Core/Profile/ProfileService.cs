using System;
using System.Net.Http;
using mehspot.Core.Dto;
using mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using Newtonsoft.Json;
using System.Threading.Tasks;
namespace Mehspot.Core.Messaging
{
    public class ProfileService
    {
        private IApplicationDataStorage _applicationDataStorage;

        public ProfileService (IApplicationDataStorage applicationDataStorage)
        {
            _applicationDataStorage = applicationDataStorage;
        }

        public Action<int, object> OnSendNotification;

        public async Task<Result<ProfileDto>> GetProfileAsync ()
        {
            var uri = new Uri ($"{Constants.ApiHost}/api/Profile/Get");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var response = await webClient.GetAsync (uri).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var data = JsonConvert.DeserializeObject<ProfileDto> (responseString);

                        return new Result<ProfileDto> {
                            IsSuccess = true,
                            Data = data,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new Result<ProfileDto> {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new Result<ProfileDto> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }
    }
}
