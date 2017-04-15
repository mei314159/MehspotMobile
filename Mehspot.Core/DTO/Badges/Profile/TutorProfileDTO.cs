namespace Mehspot.Core.DTO.Badges
{

    public class TutorProfileDTO : IBadgeProfileValues
    {
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string TutorGrade { get; set; }
        public string TutorSubject { get; set; }
        public string TutorSubjectOther { get; set; }
        public string HourlyRate { get; set; }
        public string TutorCanTravel { get; set; }
        public string TutorPreferredLocation { get; set; }
        public string TutorPreferredDate { get; set; }
        public string TutorPreferredLength { get; set; }
        public string AdditionalInfo { get; set; }

    	public string InfoLabel1
    	{
    		get { return $"${(this.HourlyRate ?? "0")}/hr"; }
    	}

    	public string InfoLabel2
    	{
    		get { return TutorCanTravel; }
        }
    }
    
}