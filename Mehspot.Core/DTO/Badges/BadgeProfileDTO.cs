using Mehspot.Core.DTO.Profile;

namespace Mehspot.Core.DTO.Badges
{
    public class BadgeProfileDTO<T> where T: IBadgeProfileValues
    {
        public string UserId { get; set; }
        public int BadgeId { get; set; }
        public string BadgeName { get; set; }
        public bool IsEnabled { get; set; }
        public T BadgeValues { get; set; }
        public BadgeProfileDetailsDTO Details { get; set; }
        public ProfilePartialDTO Profile { get; set; }
    }

    
}