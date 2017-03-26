namespace MehSpot.Models.ViewModels
{
    public class BadgeUserDetailsFilterDTO
    {
        public string UserId { get; set; }
        public int BadgeId { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public int Likes { get; set; }
        public int Recommendations { get; set; }
        public bool HasPicture { get; set; }
        public bool HasReferences { get; set; }
        public string Subdivision { get; set; }
        public int? SubdivisionId { get; set; }
        public double? DistanceFrom { get; set; }
        public bool Messaged { get; set; }
        public bool Favourite { get; set; }
        public string ProfilePicturePath { get; set; }
    }
}