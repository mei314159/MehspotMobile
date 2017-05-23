using Mehspot.Core.DTO.Search;

namespace Mehspot.Core.DTO.Badges
{

    public class OtherJobsProfileDTO : IBadgeProfileValues
    {
        string otherJobsType;

        [Cell("First Name", 0, CellType.TextView)]
        public string FirstName { get; set; }

        [Cell("Job", 1, CellType.TextView)]
        public string OtherJobsType
        {
            get
            {
                var jobs = otherJobsType.Replace(",Other", string.Empty);
                return $"{jobs},{OtherJobsOther ?? string.Empty}".Trim(',');
            }

            set
            {
                otherJobsType = value;
            }
        }

        public string OtherJobsOther { get; set; }

        [Cell("Gender", 2, CellType.TextView)]
        public string Gender { get; set; }

        [Cell("Age Range", 3, CellType.TextView)]
        public string OtherJobsAgeRange { get; set; }

        [Cell("License", 4, CellType.TextView)]
        public string OtherJobsLicense { get; set; }

        [Cell("Preferred Date and Time", 5, CellType.TextView)]
        public string OtherJobsPreferredDateTime { get; set; }

        [Cell("Experience", 6, CellType.TextView)]
        public string OtherJobsExperiencey { get; set; }

        [Cell("Reference", 7, CellType.TextView)]
        public string OtherJobsReference { get; set; }

        [Cell("Contact Info", 8, CellType.TextView)]
        public string OtherJobsAdditionalContactInfo { get; set; }

        [Cell("Additional Information", 9, CellType.TextView)]
        public string OtherJobsAdditionalInformation { get; set; }

        public string InfoLabel1
        {
            get { return Gender; }
        }

        public string InfoLabel2
        {
            get { return OtherJobsType; }
        }
    }
}