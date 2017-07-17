using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Mehspot.Core.Dto;
using Mehspot.Core.Contracts;
using Newtonsoft.Json;
using Mehspot.Core.DTO;

namespace Mehspot.Core.Auth
{
    public class AccountService : BaseDataService
    {
        public AccountService(IApplicationDataStorage applicationDataStorage) : base(applicationDataStorage)
        {
        }

        public AuthenticationInfoDTO AuthInfo
        {
            get { return this.ApplicationDataStorage.AuthInfo; }
        }

        public event Action<AuthenticationInfoDTO> Authenticated;

        public bool IsAuthenticated()
        {
            return this.ApplicationDataStorage.AuthInfo != null && (DateTime.Now - this.ApplicationDataStorage.AuthInfo.AuthDate).TotalSeconds < this.ApplicationDataStorage.AuthInfo.ExpiresIn;
        }

        public async Task<AuthenticationResult> SignInAsync(string email, string password)
        {
            var uri = new Uri(Constants.ApiHost + "/token");

            using (var webClient = new HttpClient())
            {
                try
                {
                    var data = new Dictionary<string, string>();
                    data.Add("grant_type", "password");
                    data.Add("username", email);
                    data.Add("password", password);

                    var response = await webClient.PostAsync(uri, new FormUrlEncodedContent(data)).ConfigureAwait(false);
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var authInfo = JsonConvert.DeserializeObject<AuthenticationInfoDTO>(responseString);
                        authInfo.AuthDate = DateTime.Now;
                        this.ApplicationDataStorage.AuthInfo = authInfo;

                        this.Authenticated?.Invoke(authInfo);

                        return new AuthenticationResult
                        {
                            IsSuccess = true,
                            AuthInfo = authInfo,
                            ErrorMessage = null
                        };
                    }
                    else
                    {
                        var errorResponse = JsonConvert.DeserializeObject<AuthenticationErrorDto>(responseString);
                        return new AuthenticationResult
                        {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorMessage
                        };
                    }

                }
                catch (HttpRequestException)
                {
                    return new AuthenticationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "No internet connection." // NameResolutionFailure
                    };
                }
                catch (Exception ex)
                {
                    return new AuthenticationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordDto dto)
        {
            return await PostAsync<object>("Account/ResetPassword", dto, true);
        }

        public async Task<Result> ForgotPasswordAsync(string email)
        {
            return await PostAsync<object>("Account/ForgotPassword", new { email }, true);
        }

        public async Task<AuthenticationResult> SignInExternalAsync(string accessToken, string provider)
        {
            var uri = new Uri($"{Constants.ApiHost}/api/Account/ObtainLocalAccessToken?provider={provider}&externalAccessToken={accessToken}");

            using (var webClient = new HttpClient())
            {
                try
                {
                    var response = await webClient.GetAsync(uri).ConfigureAwait(false);
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var authInfo = JsonConvert.DeserializeObject<AuthenticationInfoDTO>(responseString);
                        authInfo.AuthDate = DateTime.Now;
                        this.ApplicationDataStorage.AuthInfo = authInfo;
                        if (Authenticated != null)
                        {
                            Authenticated(authInfo);
                        }
                        return new AuthenticationResult
                        {
                            IsSuccess = true,
                            AuthInfo = authInfo,
                            ErrorMessage = null
                        };
                    }
                    else
                    {
                        var errorResponse = JsonConvert.DeserializeObject<AuthenticationErrorDto>(responseString);
                        return new AuthenticationResult
                        {
                            IsSuccess = false,
                            ErrorMessage = errorResponse.ErrorMessage
                        };
                    }

                }
                catch (HttpRequestException)
                {
                    return new AuthenticationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "No internet connection."
                    };
                }
                catch (Exception ex)
                {
                    return new AuthenticationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }

            }
        }

        public async Task<Result> SignUpAsync(string email, string username, string password, string confirmPassword)
        {
            var result = await this.PostAsync<object>("Account/Register", new SignUpDTO { Email = email, UserName = username, Password = password, ConfirmPassword = confirmPassword }, true).ConfigureAwait(false);
            return result;
        }

        public void SignOut()
        {
            this.ApplicationDataStorage.AuthInfo = null;
        }
    }
}
