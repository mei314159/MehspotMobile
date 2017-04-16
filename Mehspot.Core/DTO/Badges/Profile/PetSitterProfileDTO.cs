namespace Mehspot.Core.DTO.Badges
{

    public class PetSitterProfileDTO : IBadgeProfileValues
    {
        public string FirstName { get; set; }
        public string AgeRange { get; set; }
        public string PetSitterPetType { get; set; }
        public string PetSitterPetWeight { get; set; }
        public string PetSitterExpirience { get; set; }
        public string PetSitterExpirienceOther { get; set; }
        public bool PetSitterDropOff { get; set; }
        public string PetSitterAdditionalQualification { get; set; }
        public string PetSitterDogRate { get; set; }
        public string PetSitterCatRate { get; set; }
        public string PetSitterOtherRate { get; set; }
        public bool PetSitterMultiplePetDiscount { get; set; }
        public bool PetSitterMultipleDaysDiscount { get; set; }

        public string AdditionalInfo { get; set; }

        public string InfoLabel1
        {
            get { return string.Empty; }
        }

        public string InfoLabel2
        {
            get { return string.Empty; }
        }
    }

}