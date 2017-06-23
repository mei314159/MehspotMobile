namespace Mehspot.Core.DTO.Badges
{
    public class BadgeUserDescriptionDTO
    {
        public int BadgeId { get; set; }
        public string BadgeName { get; set; }
        public string EmployeeId { get; set; }
        public bool Delete { get; set; }
        public BadgeDescriptionTypeEnum Type { get; set; }
        public string Comment { get; set; }
    }
}