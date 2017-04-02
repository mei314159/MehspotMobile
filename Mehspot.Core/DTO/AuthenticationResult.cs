using Mehspot.Core.DTO;

namespace mehspot.Core.Auth
{
    public class AuthenticationResult
    {
        public AuthenticationInfoDTO AuthInfo { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
    }
}