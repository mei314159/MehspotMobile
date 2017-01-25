using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using mehspot.core.Auth.Dto;
using mehspot.core.Contracts;
using Mehspot.Core;
using Newtonsoft.Json;

namespace mehspot.core.Auth
{
    public class AuthenticationManager
    {
        private readonly IApplicationDataStorage _applicationDataStorage;

        public AuthenticationManager (IApplicationDataStorage applicationDataStorage)
        {
            this._applicationDataStorage = applicationDataStorage;
        }

        public bool IsAuthenticated ()
        {
            return this._applicationDataStorage.AuthInfo != null;
        }

        public async Task<AuthenticationResult> AuthenticateAsync (string email, string password)
        {
            var uri = new Uri (Constants.ApiHost + "/token");

            using (var webClient = new HttpClient ()) {
                try {
                    var data = new Dictionary<string, string> ();
                    data.Add ("grant_type", "password");
                    data.Add ("username", email);
                    data.Add ("password", password);

                    var response = await webClient.PostAsync (uri, new FormUrlEncodedContent (data)).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var authInfo = JsonConvert.DeserializeObject<AuthenticationInfoResult> (responseString);
                        _applicationDataStorage.AuthInfo = authInfo;
                        return new AuthenticationResult {
                            IsSuccess = true,
                            AuthInfo = authInfo,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorResult> (responseString);
                        return new AuthenticationResult {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new AuthenticationResult {
                        IsSuccess = true,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }
    }
}
