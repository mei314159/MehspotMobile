using System;
using System.Threading.Tasks;
using Mehspot.Core.Auth;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;

namespace Mehspot.Core.Models
{
    public class ForgotPasswordModel
    {
        private readonly AccountService authManager;
        public readonly IViewHelper ViewHelper;

        public ForgotPasswordModel(AccountService authManager, IViewHelper viewHelper)
        {
            this.authManager = authManager;
            this.ViewHelper = viewHelper;
        }

        public event Action<Result> OnSuccess;

        public async Task ForgotPasswordAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewHelper.ShowAlert("Validation error", "Please enter your email.");
            }
            else
            {
                ViewHelper.ShowOverlay("Reset Password...");
                var result = await authManager.ForgotPasswordAsync(email);
                ViewHelper.HideOverlay();

                if (result.IsSuccess)
                {
                    if (OnSuccess != null)
                    {
                        OnSuccess(result);
                    }
                }
                else if (!result.IsNetworkIssue)
                {
                    ViewHelper.ShowAlert("Authentication error", result.ErrorMessage);
                }
            }
        }
    }
}
