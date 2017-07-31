using System;
using System.Threading.Tasks;
using Mehspot.Core.Auth;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.Services;

namespace Mehspot.Core.Models
{
    public class SignInModel
    {
        private readonly AccountService accountService;
        private readonly ProfileService profileService;
        private readonly IViewHelper viewHelper;
        public static string[] FbReadPermissions = { "public_profile", "email" };
        public SignInModel(AccountService accountService, ProfileService profileService, IViewHelper viewHelper)
        {
            this.accountService = accountService;
            this.profileService = profileService;
            this.viewHelper = viewHelper;
        }

        public event Action<AuthenticationResult, ProfileDto> SignedIn;
        public event Action<AuthenticationResult> SignInError;

        public async Task SignInAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                viewHelper.ShowAlert("Validation error", "Please enter your email.");
            }
            else if (string.IsNullOrWhiteSpace(password))
            {
                viewHelper.ShowAlert("Validation error", "Please enter your password.");
            }
            else
            {
                viewHelper.ShowOverlay("Sign In...");
                var authenticationResult = await accountService.SignInAsync(email, password).ConfigureAwait(false);

                if (authenticationResult.IsSuccess)
                {
                    await this.SignedInInternalAsync(authenticationResult).ConfigureAwait(false);
                    viewHelper.HideOverlay();
                }
                else
                {
                    viewHelper.ShowAlert("Authentication error", authenticationResult.ErrorMessage);
                    viewHelper.HideOverlay();
                }
            }
        }

        public async Task SignInExternalAsync(string token, string provider)
        {
            viewHelper.ShowOverlay("Sign In...");
            var authenticationResult = await accountService.SignInExternalAsync(token, provider).ConfigureAwait(false);


            if (authenticationResult.IsSuccess)
            {
                await this.SignedInInternalAsync(authenticationResult).ConfigureAwait(false);
                viewHelper.HideOverlay();
            }
            else
            {
                SignInError?.Invoke(authenticationResult);
                viewHelper.ShowAlert("Authentication error", authenticationResult.ErrorMessage);
                viewHelper.HideOverlay();
            }
        }

        public async Task SignInExternalErrorAsync(string message)
        {
            viewHelper.ShowAlert("Authentication error", message);
        }

        async Task SignedInInternalAsync(AuthenticationResult authenticationResult)
        {
            var profile = await profileService.LoadProfileAsync().ConfigureAwait(false);
            SignedIn?.Invoke(authenticationResult, profile.Data);
        }
    }
}
