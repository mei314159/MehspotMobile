using mehspot.Core.Auth.Dto;

namespace mehspot.Core.Auth
{
    public class AuthenticationResult
    {
        public AuthenticationInfoResult AuthInfo { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
    }
}