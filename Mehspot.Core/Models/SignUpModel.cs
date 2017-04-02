using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using mehspot.Core.Auth;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;

namespace Mehspot.Core.Models
{

    public class SignUpModel
    {
        public readonly IViewHelper viewHelper;
        private readonly AccountService authManager;

        public SignUpModel(AccountService authManager, IViewHelper alertWrapper)
        {
            this.authManager = authManager;
            this.viewHelper = alertWrapper;
        }

        public event Action<Result> SignedUp;

        public async Task SignUpAsync(string email, string username, string password, string confirmPassword, bool agreeWithTerms)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                viewHelper.ShowAlert("Validation error", "Please enter your username.");
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                viewHelper.ShowAlert("Validation error", "Please enter your email.");
            }
            else if (!Regex.IsMatch(password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{6,}$"))
            {
                viewHelper.ShowAlert("Validation error", "Passwords must have at least: one digit ('0'-'9'), one uppercase ('A'-'Z'), one lowercase ('a'-'z')");
            }
            else if (password != confirmPassword)
            {
                viewHelper.ShowAlert("Validation error", "Password and Confirmation password are different.");
            }
            else if (!agreeWithTerms) {
                viewHelper.ShowAlert("Validation error", "You must acknowledge by checking the box.");
            }
            else
            {

                viewHelper.ShowOverlay("Sign Up...");
                var result = await authManager.SignUpAsync(email, username, password, confirmPassword);
                viewHelper.HideOverlay();

                if (result.IsSuccess)
                {
                    if (SignedUp != null)
                    {
                        viewHelper.ShowAlert("Sign Up", "You're successfully signed up. An email confirmation has been sent to your email address.");
                        SignedUp(result);
                    }
                }
                else
                {
                    viewHelper.ShowAlert("Sign Up error", result.ErrorMessage);
                }
            }
        }
    }
}
