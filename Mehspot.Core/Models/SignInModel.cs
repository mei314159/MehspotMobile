using System;
using System.Threading.Tasks;
using mehspot.Core.Auth;
using Mehspot.Core.Contracts.Wrappers;

namespace Mehspot.Core.Models
{
    public class SignInModel
    {
        private readonly AccountService authManager;
        private readonly IViewHelper viewHelper;


        public SignInModel(AccountService authManager, IViewHelper viewHelper)
        {
            this.authManager = authManager;
            this.viewHelper = viewHelper;
        }

        public event Action<AuthenticationResult> SignedIn;

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
                var authenticationResult = await authManager.SignInAsync(email, password);
                viewHelper.HideOverlay();

                if (authenticationResult.IsSuccess)
                {
                    if (SignedIn != null)
                    {
                        SignedIn(authenticationResult);
                    }
                }
                else
                {
                    viewHelper.ShowAlert("Authentication error", authenticationResult.ErrorMessage);
                }
            }
        }
    }
}
