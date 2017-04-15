namespace Mehspot.Core.DTO.Badges
{

    public class FitnessProfileDTO : IBadgeProfileValues
    {
        public string FirstName { get; set; }
        public string FitnessType { get; set; }
        public string FitnessOther { get; set; }
        public string Gender { get; set; }
        public string FitnessAgeRange { get; set; }
        public string FitnessPreferredDateTime { get; set; }
        public string FitnessPreferredFrequency { get; set; }
        public string FitnessPreferredLocation { get; set; }
        public string FitnessPreferredGender { get; set; }
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