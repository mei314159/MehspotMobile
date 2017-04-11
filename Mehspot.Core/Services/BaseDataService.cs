
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using mehspot.Core.Contracts;
using mehspot.Core.Dto;
using Mehspot.Core;
using Mehspot.Core.DTO;
using Newtonsoft.Json;

namespace mehspot.Core
{
    public class BaseDataService
    {
        protected readonly IApplicationDataStorage ApplicationDataStorage;

        public BaseDataService (IApplicationDataStorage _applicationDataStorage)
        {
            this.ApplicationDataStorage = _applicationDataStorage;
        }

        public async Task<Result<T>> GetAsync<T> (string uri)
        {
            var requestUri = new Uri ($"{Constants.ApiHost}/api/{uri}");

            using (var webClient = new HttpClient ()) {
                try {
                    webClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Bearer", this.ApplicationDataStorage.AuthInfo.AccessToken);

                    var response = await webClient.GetAsync (requestUri).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var data = JsonConvert.DeserializeObject<T> (responseString);

                        return new Result<T> {
                            IsSuccess = true,
                            Data = data,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new Result<T> {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorMessage
                        };
                    }

                } catch (Exception ex) {
                    MehspotAppContext.Instance.LogException(ex);
                    return new Result<T> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        public Task<Result<TResult>> PutAsync<TResult> (string uri, object data)
        {
            return SendDataAsync<TResult> (uri, data, HttpMethod.Put);
        }

        public Task<Result<TResult>> PostAsync<TResult> (string uri, object data, bool anonymously = false)
        {
            return SendDataAsync<TResult> (uri, data, HttpMethod.Post, anonymously);
        }

        public Task<Result<TResult>> SendDataAsync<TResult> (string uri, object data, HttpMethod method, bool anonymously = false) {

            var serializedData = JsonConvert.SerializeObject (data);
            var stringContent = new StringContent (serializedData, System.Text.Encoding.UTF8, "application/json");
            stringContent.Headers.ContentLength = serializedData.Length;

            return SendDataAsync<TResult> (uri, method, stringContent, anonymously);
        }

        public async Task<Result<TResult>> SendDataAsync<TResult> (string uri, HttpMethod method, HttpContent content, bool anonymously = false)
        {
            var requestUri = new Uri ($"{Constants.ApiHost}/api/{uri}");

            using (var webClient = new HttpClient ()) {
                try {
                    if (!anonymously)
                    {
                        webClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.ApplicationDataStorage.AuthInfo.AccessToken);
                    }

                    HttpResponseMessage response;
                    if (method == HttpMethod.Put)
                        response = await webClient.PutAsync (requestUri, content).ConfigureAwait (false);
                    else if (method == HttpMethod.Post)
                        response = await webClient.PostAsync (requestUri, content).ConfigureAwait (false);
                    else {
                        throw new ArgumentException ("HttpMethod is not supported", nameof (method));
                    }

                    string responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var messageDto = JsonConvert.DeserializeObject<TResult> (responseString);
                        return new Result<TResult> {
                            IsSuccess = true,
                            Data = messageDto,
                            ErrorMessage = null
                        };
                    } else {
                        var modelState = JsonConvert.DeserializeObject<ModelStateDto> (responseString);
                        return new Result<TResult> {
                            IsSuccess = false,
                            ErrorMessage = modelState.Message,
                            ModelState = modelState
                        };
                    }

                } catch (Exception ex) {
                    return new Result<TResult> {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }
    }
}