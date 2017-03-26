namespace MehSpot.Models.ViewModels
{
    public class BabysitterSearchResultDTO
    {
        public BadgeUserDetailsFilterDTO Details { get; set; }
        public string Description { get; set; }
        public double? HourlyRate { get; set; }
        public int? AgeRange { get; set; }
        public bool OwnCar { get; set; }
        public bool CanDrive { get; set; }
        public string Location { get; set; }
        public string BabysitterInformation { get; set; }
        public bool IsHired { get; set; }
        public string SubdivisionInfo { get; set; }
        public string Certification { get; set; }
        public bool HasCertification { get; set; }
    }
}