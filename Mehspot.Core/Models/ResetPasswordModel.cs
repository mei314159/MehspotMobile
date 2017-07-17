using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mehspot.Core.Auth;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.Services;

namespace Mehspot.Core.Models
{

    public class ResetPasswordModel : SignInModel
    {
        private readonly AccountService authManager;
        private readonly IViewHelper viewHelper;

        public ResetPasswordModel(AccountService authManager, ProfileService profileService, IViewHelper viewHelper) : base(authManager, profileService, viewHelper)
        {
            this.authManager = authManager;
            this.viewHelper = viewHelper;
        }

        public event Action<Result> OnResetPasswordSuccess;

        public async Task ResetPasswordAsync(string email, string code, string password, string confirmPassword)
        {
            if (!Regex.IsMatch(password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{6,}$"))
            {
                viewHelper.ShowAlert("Validation error", "Passwords must have at least: one digit ('0'-'9'), one uppercase ('A'-'Z'), one lowercase ('a'-'z'), one special character");
            }
            else if (password != confirmPassword)
            {
                viewHelper.ShowAlert("Validation error", "Password and Confirmation password are different.");
            }
            else
            {
                viewHelper.ShowOverlay("Reset Password...");
                var dto = new ResetPasswordDto
                {
                    Email = email,
                    Code = code,
                    Password = password,
                    ConfirmPassword = confirmPassword
                };
                var result = await authManager.ResetPasswordAsync(dto);
                viewHelper.HideOverlay();

                if (result.IsSuccess)
                {
                    if (OnResetPasswordSuccess != null)
                    {
                        OnResetPasswordSuccess(result);
                    }
                }
                else
                {
                    viewHelper.ShowAlert("Authentication error", result.ErrorMessage);
                }
            }
        }
    }
}
