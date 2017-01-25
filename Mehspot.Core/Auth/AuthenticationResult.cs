using mehspot.core.Auth.Dto;
using mehspot.core.Contracts;

namespace mehspot.core.Auth
{
    public class AuthenticationResult
    {
        public AuthenticationInfoResult AuthInfo { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
    }
}