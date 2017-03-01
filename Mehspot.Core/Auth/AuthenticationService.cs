using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using mehspot.Core.Dto;
using mehspot.Core.Contracts;
using Mehspot.Core;
using Newtonsoft.Json;
using Mehspot.Core.DTO;

namespace mehspot.Core.Auth
{
    public class AuthenticationService
    {
        private readonly IApplicationDataStorage _applicationDataStorage;

        public AuthenticationService (IApplicationDataStorage applicationDataStorage)
        {
            this._applicationDataStorage = applicationDataStorage;
        }

        public AuthenticationInfoDto AuthInfo 
        {
            get { return _applicationDataStorage.AuthInfo; }
        }

        public event Action<AuthenticationInfoDto> Authenticated;

        public bool IsAuthenticated ()
        {
            return _applicationDataStorage.AuthInfo != null && (DateTime.Now - _applicationDataStorage.AuthInfo.AuthDate).TotalSeconds < _applicationDataStorage.AuthInfo.ExpiresIn;
        }

        public async Task<AuthenticationResult> SignInAsync (string email, string password)
        {
            var uri = new Uri (Constants.AuthServerHost + "/token");

            using (var webClient = new HttpClient ()) {
                try {
                    var data = new Dictionary<string, string> ();
                    data.Add ("grant_type", "password");
                    data.Add ("username", email);
                    data.Add ("password", password);

                    var response = await webClient.PostAsync (uri, new FormUrlEncodedContent (data)).ConfigureAwait (false);
                    var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        var authInfo = JsonConvert.DeserializeObject<AuthenticationInfoDto> (responseString);
                        authInfo.AuthDate = DateTime.Now;
                        _applicationDataStorage.AuthInfo = authInfo;
                        if (Authenticated != null) {
                            Authenticated (authInfo);
                        }
                        return new AuthenticationResult {
                            IsSuccess = true,
                            AuthInfo = authInfo,
                            ErrorMessage = null
                        };
                    } else {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorDto> (responseString);
                        return new AuthenticationResult {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorDescription
                        };
                    }

                } catch (Exception ex) {
                    return new AuthenticationResult {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        public void SignOut()
        {
            _applicationDataStorage.AuthInfo = null;
        }
    }
}
