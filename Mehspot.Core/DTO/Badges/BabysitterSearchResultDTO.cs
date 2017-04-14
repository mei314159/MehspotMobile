using System;

namespace MehSpot.Models.ViewModels
{
    public class BabysitterSearchResultDTO : ISearchResultDTO
    {
        public BadgeUserDetailsFilterDTO Details { get; set; }
        public string Description { get; set; }
        public double? HourlyRate { get; set; }
        public int? AgeRange { get; set; }
        public string AgeRangeLabel { get; set; }
        public bool OwnCar { get; set; }
        public bool CanDrive { get; set; }
        public string Location { get; set; }
        public string BabysitterInformation { get; set; }
        public bool IsHired { get; set; }
        public string SubdivisionInfo { get; set; }
        public string Certification { get; set; }
        public bool HasCertification { get; set; }

        public string InfoLabel1
        {
            get { return $"${(this.HourlyRate ?? 0)}/hr"; }
        }

        public string InfoLabel2
        {
            get { return this.AgeRangeLabel; }
        }
    }
}