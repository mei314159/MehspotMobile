using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Badges
{
    [ViewProfileDto(Constants.BadgeNames.PetSitter)]
    public class PetSitterProfileDTO : IBadgeProfileValues
    {
        [Cell("First Name", 0, CellType.TextView)]
        public string FirstName { get; set; }

        [Cell("Age Range", 1, CellType.TextView)]
        public string AgeRange { get; set; }

        [Cell("Types of pet willing to sit", 2, CellType.TextView)]
        public string PetSitterPetType { get; set; }

        [Cell("For dogs size up to, pounds", 3, CellType.TextView)]
        public string PetSitterPetWeight { get; set; }

        [Cell("Experience With", 4, CellType.TextView)]
        public string PetSitterExpirience { get; set; }

        [Cell("Experience other", 5, CellType.TextView)]
        public string PetSitterExpirienceOther { get; set; }

        [Cell("Drop off option", 6, CellType.Boolean, ReadOnly = true)]
        public bool PetSitterDropOff { get; set; }

        [Cell("Additional Qualification", 7, CellType.TextView)]
        public string PetSitterAdditionalQualification { get; set; }

        [Cell("Dog Rate", 7, CellType.TextView)]
        public string PetSitterDogRate { get; set; }

        [Cell("Cat Rate", 8, CellType.TextView)]
        public string PetSitterCatRate { get; set; }

        [Cell("Other Rate", 9, CellType.TextView)]
        public string PetSitterOtherRate { get; set; }

        [Cell("Multiple pet discount", 10, CellType.Boolean, ReadOnly = true)]
        public bool PetSitterMultiplePetDiscount { get; set; }

        [Cell("Multiple day discount", 11, CellType.Boolean, ReadOnly = true)]
        public bool PetSitterMultipleDaysDiscount { get; set; }

        [Cell("Additional Information", 12, CellType.TextView)]
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