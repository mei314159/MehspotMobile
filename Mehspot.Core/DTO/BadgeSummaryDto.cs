namespace Mehspot.Core.DTO
{
    public class BadgeSummaryDto
    {
        public string BadgeName { get; set; }
        public int BadgeId { get; set; }
        public int Likes { get; set; }
        public int Recommendations { get; set; }
        public int References { get; set; }
        public bool IsRegistered { get; set; }
        public string ImageName { get; set; }
    }
}