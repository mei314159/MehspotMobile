using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mehspot.Core.Auth;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;

namespace Mehspot.Core.Models
{

    public class SignUpModel: SignInModel
    {
        public readonly IViewHelper viewHelper;
        private readonly AccountService authManager;

        public SignUpModel(AccountService authManager, IViewHelper viewHelper): base( authManager, viewHelper)
        {
            this.authManager = authManager;
            this.viewHelper = viewHelper;
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
            else if (!Regex.IsMatch(password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{6,}$"))
            {
                viewHelper.ShowAlert("Validation error", "Passwords must have at least: one digit ('0'-'9'), one uppercase ('A'-'Z'), one lowercase ('a'-'z'), one special character");
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
                        SignedUp(result);
                    }
                }
                else
                {
                    var modelStateError = result.ModelState?.ModelState?.Count > 0 ? result.ModelState.ModelState.First().Value.FirstOrDefault() : result.ErrorMessage;
                    viewHelper.ShowAlert("Sign Up error", modelStateError);
                }
            }
        }
    }
}
