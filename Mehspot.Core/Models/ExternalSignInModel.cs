using System;
using System.Threading.Tasks;
using mehspot.Core.Auth;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;

namespace Mehspot.Core.Models
{

    public class ExternalSignInModel
    {
        private readonly AccountService authManager;
        private readonly IExternalSignInController signInController;

        public ExternalSignInModel(AccountService authManager, IExternalSignInController signInController)
        {
            this.authManager = authManager;
            this.signInController = signInController;
        }

        public event Action<AuthenticationResult> SignedIn;

        public void BeginExternalSignIn()
        {
            this.signInController.LoadExternalSignInPage();
        }

        public async Task SignInExternalAsync(string token, string provider)
        {
            signInController.ViewHelper.ShowOverlay("Sign In...");
            var authenticationResult = await authManager.SignInExternalAsync(token, provider);
            signInController.ViewHelper.HideOverlay();

            if (authenticationResult.IsSuccess)
            {
                if (SignedIn != null)
                {
                    SignedIn(authenticationResult);
                }
            }
            else
            {
                signInController.ViewHelper.ShowAlert("Authentication error", authenticationResult.ErrorMessage);
            }

        }
    }
}
