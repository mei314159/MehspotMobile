using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Mehspot.Core.Contracts;
using Mehspot.Core.Dto;
using Mehspot.Core.DTO;
using Newtonsoft.Json;

namespace Mehspot.Core.Push
{
    public class PushService
    {
        private IApplicationDataStorage _applicationDataStorage;

        public PushService (IApplicationDataStorage applicationDataStorage)
        {
            _applicationDataStorage = applicationDataStorage;
        }

        public async Task<Result> RegisterAsync (string oldToken, string newToken)
        {
            var uri = new Uri (Constants.ApiHost + "/api/Push/RegisterToken");

            using (var webClient = new HttpClient ()) {
                try
                {
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this._applicationDataStorage.AuthInfo.AccessToken);

                    var data = new Dictionary<string, string>();
                    data.Add("oldToken", oldToken);
                    data.Add("token", newToken);
                    data.Add("osType", _applicationDataStorage.OsType.ToString());

                    var response = await webClient.PostAsync(uri, new FormUrlEncodedContent(data)).ConfigureAwait(false);
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return new Result
                        {
                            IsSuccess = true,
                            ErrorMessage = null
                        };
                    }

                    var errorResponse = JsonConvert.DeserializeObject<ErrorDto>(responseString);
                    return new Result
                    {
                        IsSuccess = false,
                        ErrorMessage = errorResponse.ErrorMessage
                    };

                }
                catch (WebException ex)
				{
                    MehspotAppContext.Instance.LogNetworkException(ex);
					return new Result
					{
						IsSuccess = false,
						ErrorMessage = ex.Message,
                        IsNetworkIssue = true
					};
                }
                catch (Exception ex) {
                    return new Result {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }
    }
}