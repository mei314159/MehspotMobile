using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Badges
{
    [ViewProfileDto(Constants.BadgeNames.TutorEmployer)]
    public class TutorEmployerProfileDTO : IBadgeProfileValues
    {
        [Cell("First Name", 0, CellType.TextView)]
        public string FirstName { get; set; }

        [Cell("Gender", 1, CellType.TextView)]
        public string Gender { get; set; }

        [Cell("Student Grade", 2, CellType.TextView)]
        public string TutorEmployerGrade { get; set; }

        [Cell("Subject", 3, CellType.TextView)]
        public string TutorEmployerSubject { get; set; }

        [Cell("Other Subject", 4, CellType.TextView)]
        public string TutorEmployerSubjectOther { get; set; }

        [Cell("Can travel to locations", 5, CellType.TextView)]
        public string TutorEmployerCanTravel { get; set; }

        [Cell("Preferred Location", 6, CellType.TextView)]
        public string TutorEmployerPreferredLocation { get; set; }

        [Cell("Preferred Date", 7, CellType.TextView)]
        public string TutorEmployerPreferredDate { get; set; }

        [Cell("Preferred Length", 8, CellType.TextView)]
        public string TutorEmployerPreferredLength { get; set; }

        [Cell("Additional Information", 9, CellType.TextView)]
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