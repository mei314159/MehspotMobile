using System;
using System.Net.Http;
using mehspot.Core.Dto;
using mehspot.Core.Contracts;
using mehspot.Core.Messaging;
using Mehspot.Core.DTO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Mehspot.Core.Messaging
{
    public class MessagesService
    {
        private IApplicationDataStorage _applicationDataStorage;
        
        public MessagesService (IApplicationDataStorage applicationDataStorage)
        {
            _applicationDataStorage = applicationDataStorage;
        }

        public async Task<MessagesResult> GetMessages (string toUserName, int pageNumber)
        {
            var uri = new Uri ($"{Constants.ApiHost}/api/Badges/GetMessagesByUserName?toUserName={toUserName}&pageNumber={pageNumber}");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var response = await webClient.GetAsync (uri).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var messages = JsonConvert.DeserializeObject<CollectionDto<MessageDto>> (responseString);

                        return new MessagesResult {
                            IsSuccess = true,
                            Messages = messages,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<AuthErrorDto> (responseString);
                        return new MessagesResult {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new MessagesResult {
                        IsSuccess = true,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }
    }
}
