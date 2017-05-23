using Mehspot.Core.DTO.Search;

namespace Mehspot.Core.DTO.Badges
{

    public class FitnessProfileDTO : IBadgeProfileValues
    {
		string fitnessType;

        [Cell("First Name", 0, CellType.TextView)]
        public string FirstName { get; set; }

        [Cell("Gender", 1, CellType.TextView)]
        public string Gender { get; set; }

        [Cell("Fitness Type", 2, CellType.TextView)]
        public string FitnessType
        {
            get
            {
                var jobs = fitnessType.Replace(",Other", string.Empty);
                return $"{jobs},{FitnessOther ?? string.Empty}".Trim(',');
            }
            set
            {
                fitnessType = value;
            }
        }

        public string FitnessOther { get; set; }

        [Cell("Age Range", 3, CellType.TextView)]
        public string FitnessAgeRange { get; set; }

        [Cell("Preferred Date", 4, CellType.TextView)]
        public string FitnessPreferredDateTime { get; set; }

        [Cell("Preferred Frequency", 5, CellType.TextView)]
        public string FitnessPreferredFrequency { get; set; }

        [Cell("Preferred Location", 6, CellType.TextView)]
        public string FitnessPreferredLocation { get; set; }

        [Cell("Preferred Gender", 7, CellType.TextView)]
        public string FitnessPreferredGender { get; set; }

        [Cell("Additional Information", 8, CellType.TextView)]
        public string FitnessAdditionalInformation { get; set; }

        public string InfoLabel1
        {
            get { return Gender; }
        }

        public string InfoLabel2
        {
            get { return string.Empty; }
        }
    }
}