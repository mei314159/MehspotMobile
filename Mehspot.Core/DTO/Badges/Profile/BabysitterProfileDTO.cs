using Mehspot.Core.DTO.Search;

namespace Mehspot.Core.DTO.Badges
{

    public class BabysitterProfileDTO : IBadgeProfileValues
    {
        public string FirstName { get; set; }
        public double? HourlyRate { get; set; }
        public string AgeRange { get; set; }

        [Cell("Own Car", 0, CellType.Boolean, ReadOnly = true)]
        public bool OwnCar { get; set; }

        [Cell("Can Drive", 1, CellType.Boolean, ReadOnly = true)]
        public bool CanDrive { get; set; }

        [Cell("Certifications", 2, CellType.TextView)]
        public string BabysitterCertificationInfo { get; set; }

        [Cell("Other Certifications and  URLs", 3, CellType.TextView)]
        public string BabysitterOtherCertifications { get; set; }

        [Cell("Additional Information", 4, CellType.TextView)]
        public string BabysitterAdditionalInformation { get; set; }

        public string InfoLabel1
        {
            get { return $"${(this.HourlyRate ?? 0)}/hr"; }
        }

        public string InfoLabel2
        {
            get { return this.AgeRange; }
        }
    }
}