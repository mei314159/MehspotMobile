namespace Mehspot.Core.DTO.Badges
{

    public class TutorEmployerProfileDTO : IBadgeProfileValues
    {
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string TutorEmployerGrade { get; set; }
        public string TutorEmployerSubject { get; set; }
        public string TutorEmployerSubjectOther { get; set; }
        public string TutorEmployerCanTravel { get; set; }
        public string TutorEmployerPreferredLocation { get; set; }
        public string TutorEmployerPreferredDate { get; set; }
        public string TutorEmployerPreferredLength { get; set; }
        public string AdditionalInfo { get; set; }

    	public string InfoLabel1
    	{
            get { return "Grade: " + TutorEmployerGrade; }
    	}

    	public string InfoLabel2
    	{
            get { return TutorEmployerSubject; }
        }
    }
}