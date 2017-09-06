using Mehspot.Core.Push;

namespace Mehspot.Core.DTO
{

    public class SignUpDTO
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public OsType OsType { get; set; }
    }

    
}