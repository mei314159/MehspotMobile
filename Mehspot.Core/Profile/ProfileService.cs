using System;
using System.Net.Http;
using mehspot.Core.Dto;
using mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Mehspot.DTO;
using System.Net.Http.Headers;

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

        public async Task<Result<EditProfileDto>> GetProfileAsync ()
        {
            var uri = new Uri ($"{Constants.ApiHost}/api/Profile/Get");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var response = await webClient.GetAsync (uri).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var data = JsonConvert.DeserializeObject<EditProfileDto> (responseString);

                        return new Result<EditProfileDto> {
                            IsSuccess = true,
                            Data = data,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new Result<EditProfileDto> {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new Result<EditProfileDto> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        public async Task<Result<EditProfileDto>> UpdateAsync (EditProfileDto profile)
        {
            var uri = new Uri (Constants.ApiHost + "/api/profile/update");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var data = JsonConvert.SerializeObject (profile);
                    var stringContent = new StringContent (data, System.Text.Encoding.UTF8, "application/json");
                    stringContent.Headers.ContentLength = data.Length;
                    var response = await webClient.PutAsync (uri, stringContent).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var messageDto = JsonConvert.DeserializeObject<EditProfileDto> (responseString);
                        return new Result<EditProfileDto> {
                            IsSuccess = true,
                            Data = messageDto,
                            ErrorMessage = null
                        };
                    } else {
                        var modelState = JsonConvert.DeserializeObject<ModelStateDto> (responseString);
                        return new Result<EditProfileDto> {
                            IsSuccess = false,
                            ErrorMessage = modelState.Message,
                            ModelState = modelState
                        };
                    }

                } catch (Exception ex) {
                    return new Result<EditProfileDto> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        public async Task<Result> UploadProfileImageAsync (byte[] profileImage)
        {
            var uri = new Uri (Constants.ApiHost + "/api/profile/UploadProfileImage");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var data = new MultipartFormDataContent ();
                    var byteArrayContent = new ByteArrayContent (profileImage);
                    data.Add (byteArrayContent, "file", "file.jpg");
                    var response = await webClient.PostAsync (uri, data).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        return new Result {
                            IsSuccess = true,
                            ErrorMessage = null
                        };
                    } else {
                        var modelState = JsonConvert.DeserializeObject<ModelStateDto> (responseString);
                        return new Result {
                            IsSuccess = false,
                            ErrorMessage = modelState.Message,
                            ModelState = modelState
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

        public async Task<Result<StaticDataDto[]>> GetStatesAsync ()
        {
            var uri = new Uri ($"{Constants.ApiHost}/api/Profile/GetStates");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var response = await webClient.GetAsync (uri).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var data = JsonConvert.DeserializeObject<StaticDataDto[]> (responseString);

                        return new Result<StaticDataDto[]> {
                            IsSuccess = true,
                            Data = data,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new Result<StaticDataDto[]> {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new Result<StaticDataDto[]> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        public async Task<Result<SubdivisionDto []>> GetSubdivisionsAsync (string zip)
        {
            var uri = new Uri ($"{Constants.ApiHost}/api/Profile/GetSubdivisions?zipCode=" + zip);

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var response = await webClient.GetAsync (uri).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var data = JsonConvert.DeserializeObject<SubdivisionDto []> (responseString);

                        return new Result<SubdivisionDto []> {
                            IsSuccess = true,
                            Data = data,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new Result<SubdivisionDto []> {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new Result<SubdivisionDto []> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }
    }
}
