namespace Mehspot.Core.DTO.Badges
{

    public class BadgeBadgeItemDTO
    {
        public int Id { get; set; }

        public int BadgeId { get; set; }

        public int BadgeItemId { get; set; }

        public bool Required { get; set; }

        public int Order { get; set; }

        public virtual BadgeItemDTO BadgeItem { get; set; }
    }
}