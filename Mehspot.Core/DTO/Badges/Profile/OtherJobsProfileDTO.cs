namespace Mehspot.Core.DTO.Badges
{

public class OtherJobsProfileDTO : IBadgeProfileValues
{
        public string FirstName { get; set; }
        public string OtherJobsType { get; set; }
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