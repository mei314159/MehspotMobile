namespace Mehspot.Core.DTO.Badges
{

    public class BadgeItemValueDTO
    {
        public int Id { get; set; }

        public int BadgeBadgeItemId { get; set; }

        public string UserId { get; set; }

        public string Value { get; set; }

        public string[] Values { get; set; }

        public virtual BadgeBadgeItemDTO BadgeBadgeItem { get; set; }
    }
    
}