using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Badges
{
    [ViewProfileDto(Constants.BadgeNames.Tutor)]
    public class TutorProfileDTO : IBadgeProfileValues
    {
        [Cell("First Name", 0, CellType.TextView)]
        public string FirstName { get; set; }

        [Cell("Gender", 1, CellType.TextView)]
        public string Gender { get; set; }

        [Cell("Tutor Grade", 2, CellType.TextView)]
        public string TutorGrade { get; set; }

        [Cell("Subject", 3, CellType.TextView)]
        public string TutorSubject { get; set; }

        [Cell("Other subjects or language", 4, CellType.TextView)]
        public string TutorSubjectOther { get; set; }

        [Cell("Hourly Rate", 5, CellType.TextView)]
        public string HourlyRate { get; set; }

        [Cell("Can travel to locations", 6, CellType.TextView)]
        public string TutorCanTravel { get; set; }

        [Cell("Preferred Location", 7, CellType.TextView)]
        public string TutorPreferredLocation { get; set; }

        [Cell("Preferred Date", 8, CellType.TextView)]
        public string TutorPreferredDate { get; set; }

        [Cell("Preferred Length", 9, CellType.TextView)]
        public string TutorPreferredLength { get; set; }

        [Cell("Additional Information", 10, CellType.TextView)]
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