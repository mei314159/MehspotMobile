using System;
using System.Net.Http;
using mehspot.Core.Dto;
using mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Mehspot.Core.Messaging
{
    public class MessagesService
    {
        private IApplicationDataStorage _applicationDataStorage;

        public MessagesService (IApplicationDataStorage applicationDataStorage)
        {
            _applicationDataStorage = applicationDataStorage;
        }

        public Action<int, object> OnSendNotification;

        public async Task<Result<MessageBoardItemDto[]>> GetMessageBoard (string filter)
        {
            var uri = new Uri ($"{Constants.ApiHost}/api/Badges/MessageBoard?filter=" + filter);

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var response = await webClient.GetAsync (uri).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var data = JsonConvert.DeserializeObject<MessageBoardItemDto[]> (responseString);

                        return new Result<MessageBoardItemDto[]> {
                            IsSuccess = true,
                            Data = data,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new Result<MessageBoardItemDto[]> {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new Result<MessageBoardItemDto[]> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        public async Task<Result<CollectionDto<MessageDto>>> GetMessages(int pageNumber, string toUserId = null, string toUserName = null)
        {
            Uri uri = toUserId != null
                ? new Uri ($"{Constants.ApiHost}/api/Badges/GetMessages?toUserId={toUserId}&pageNumber={pageNumber}")
                : new Uri ($"{Constants.ApiHost}/api/Badges/GetMessagesByUserName?toUserName={toUserName}&pageNumber={pageNumber}");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var response = await webClient.GetAsync (uri).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var messages = JsonConvert.DeserializeObject<CollectionDto<MessageDto>> (responseString);

                        return new Result<CollectionDto<MessageDto>> {
                            IsSuccess = true,
                            Data = messages,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new Result<CollectionDto<MessageDto>> {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new Result<CollectionDto<MessageDto>> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        public async Task<Result<MessageDto>> SendMessageAsync (string message, string toUserId = null, string toUserName = null)
        {
            var uri = new Uri (Constants.ApiHost + "/api/Badges/Send");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var data = new Dictionary<string, string> ();
                    data.Add ("Message", message);
                    if (toUserId != null) {
                        data.Add ("ToUserId", toUserId);
                    } else {
                        data.Add ("ToUserName", toUserName);
                    }
                    data.Add ("FromUserId", _applicationDataStorage.AuthInfo.UserId);

                    var response = await webClient.PostAsync (uri, new FormUrlEncodedContent (data)).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var messageDto = JsonConvert.DeserializeObject<MessageDto> (responseString);
                        return new Result<MessageDto> {
                            IsSuccess = true,
                            Data = messageDto,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new Result<MessageDto> {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new Result<MessageDto> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }


        public async Task<Result> MarkMessagesReadAsync (string toUserId)
        {
            var uri = new Uri (Constants.ApiHost + "/api/Badges/MarkMessagesRead");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var data = new Dictionary<string, string> ();
                    data.Add ("toUserId", toUserId);

                    var response = await webClient.PostAsync (uri, new FormUrlEncodedContent (data)).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        return new Result {
                            IsSuccess = true,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new Result {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new Result {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }
    }
}
