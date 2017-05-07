using System;
using System.Threading.Tasks;
using mehspot.Core.Auth;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;

namespace Mehspot.Core.Models
{

    public class ResetPasswordModel
    {
        private readonly AccountService authManager;
        private readonly IViewHelper viewHelper;

        public ResetPasswordModel(AccountService authManager, IViewHelper viewHelper)
        {
            this.authManager = authManager;
            this.viewHelper = viewHelper;
        }

        public event Action<Result> OnSuccess;

        public async Task ResetPasswordAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                viewHelper.ShowAlert("Validation error", "Please enter your email.");
            }
            else
            {
                viewHelper.ShowOverlay("Reset Password...");
                var authenticationResult = await authManager.ResetPasswordAsync(email);
                viewHelper.HideOverlay();

                if (authenticationResult.IsSuccess)
                {
                    if (OnSuccess != null)
                    {
                        OnSuccess(authenticationResult);
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
