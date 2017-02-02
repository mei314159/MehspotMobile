using mehspot.Core.Dto;

namespace mehspot.Core.Auth
{
    public class AuthenticationResult
    {
        public AuthenticationInfoDto AuthInfo { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
    }
}