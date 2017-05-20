namespace Mehspot.Core.DTO.Badges
{

public class OtherJobsProfileDTO : IBadgeProfileValues
{
        public string FirstName { get; set; }
        string otherJobsType;

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

        public string Gender { get; set; }
        public string OtherJobsAgeRange { get; set; }
        public string OtherJobsLicense { get; set; }
        public string OtherJobsPreferredDateTime { get; set; }
        public string OtherJobsExperiencey { get; set; }
        public string OtherJobsReference { get; set; }
        public string OtherJobsAdditionalContactInfo { get; set; }
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