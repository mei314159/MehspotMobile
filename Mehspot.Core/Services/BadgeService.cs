using System;
using System.Net.Http;
using mehspot.Core.Dto;
using mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Extensions;
using System.Net.Http.Headers;
using MehSpot.Models.ViewModels;

namespace Mehspot.Core.Messaging
{
    public class BadgeService
    {
        private IApplicationDataStorage _applicationDataStorage;

        public BadgeService (IApplicationDataStorage applicationDataStorage)
        {
            _applicationDataStorage = applicationDataStorage;
        }

        public Action<int, object> OnSendNotification;

        public async Task<Result<BadgeSummaryDTO[]>> GetBadgesSummaryAsync ()
        {
            var uri = new Uri ($"{Constants.ApiHost}/api/Badges/Get");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var response = await webClient.GetAsync (uri).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var data = JsonConvert.DeserializeObject<BadgeSummaryDTO []> (responseString);

                        return new Result<BadgeSummaryDTO []> {
                            IsSuccess = true,
                            Data = data,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new Result<BadgeSummaryDTO []> {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new Result<BadgeSummaryDTO []> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        public async Task<Result<StaticDataDto []>> GetAgeRangesAsync (string badgeName)
        {
            var uri = new Uri ($"{Constants.ApiHost}/api/Badges/GetAgeRanges?badgeName=" + badgeName);

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var response = await webClient.GetAsync (uri).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var data = JsonConvert.DeserializeObject<StaticDataDto []> (responseString);

                        return new Result<StaticDataDto []> {
                            IsSuccess = true,
                            Data = data,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new Result<StaticDataDto []> {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new Result<StaticDataDto []> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        public async Task<Result> ToggleBadgeEmploymentHistoryAsync (string userId, string badgeName, bool delete)
        {
            var uri = new Uri (Constants.ApiHost + "/api/badges/ToggleBadgeEmploymentHistory");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var data = JsonConvert.SerializeObject (new { EmployeeId = userId, Delete = delete, BadgeName = badgeName });

                    var stringContent = new StringContent (data, System.Text.Encoding.UTF8, "application/json");
                    stringContent.Headers.ContentLength = data.Length;
                    var response = await webClient.PostAsync (uri, stringContent).ConfigureAwait (false);
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

        public async Task<Result> ToggleReferenceAsync (string userId, string badgeName, bool delete)
        {
            var uri = new Uri (Constants.ApiHost + "/api/badges/ToggleReference");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var data = JsonConvert.SerializeObject (new { EmployeeId = userId, Delete = delete, BadgeName = badgeName });

                    var stringContent = new StringContent (data, System.Text.Encoding.UTF8, "application/json");
                    stringContent.Headers.ContentLength = data.Length;
                    var response = await webClient.PostAsync (uri, stringContent).ConfigureAwait (false);
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

        public async Task<Result<TResult []>> Search<TResult>(ISearchFilterDTO filter, string badgeName, int skip, int take)
        {
            var uri = new Uri ($"{Constants.ApiHost}/api/Badges/SearchForApp?badgeName={badgeName}&skip={skip}&take={take}&" + filter.GetQueryString ());

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var response = await webClient.GetAsync (uri).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var data = JsonConvert.DeserializeObject<TResult []> (responseString);

                        return new Result<TResult []> {
                            IsSuccess = true,
                            Data = data,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new Result<TResult []> {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new Result<TResult []> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        public async Task<Result<BadgeProfileDTO<BabysitterProfileDTO>>> GetBadgeProfileAsync (string badgeName, string userId)
        {
            var uri = new Uri ($"{Constants.ApiHost}/api/Badges/Profile?badgeName={badgeName}&userId={userId}");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var response = await webClient.GetAsync (uri).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var data = JsonConvert.DeserializeObject<BadgeProfileDTO<BabysitterProfileDTO>> (responseString);

                        return new Result<BadgeProfileDTO<BabysitterProfileDTO>> {
                            IsSuccess = true,
                            Data = data,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new Result<BadgeProfileDTO<BabysitterProfileDTO>> {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new Result<BadgeProfileDTO<BabysitterProfileDTO>> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        public class BadgeNames {
            public const string Babysitter = "Babysitter";
        }
    }
}
