using Mehspot.Core.DTO;

namespace Mehspot.Core.Auth
{
    public class AuthenticationResult:Result
    {
        public AuthenticationInfoDTO AuthInfo { get; set; }
    }
}