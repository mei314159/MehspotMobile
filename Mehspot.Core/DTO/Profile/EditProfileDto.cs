namespace Mehspot.Core.DTO
{

    public class EditProfileDto : UserProfileDto
    {
        public string Email { get; set; }

        public string ReferralCode { get; set; }
    }
}
