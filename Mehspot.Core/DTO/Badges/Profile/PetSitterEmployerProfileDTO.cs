using Mehspot.Core.DTO.Search;

namespace Mehspot.Core.DTO.Badges
{

    public class PetSitterEmployerProfileDTO : IBadgeProfileValues
    {
        [Cell("First Name", 0, CellType.TextView)]
        public string FirstName { get; set; }

        [Cell("Types of pet", 1, CellType.TextView)]
        public string PetSitterEmployerPetType { get; set; }

        [Cell("For dogs size up to, pounds", 2, CellType.TextView)]
        public string PetSitterEmployerPetWeight { get; set; }

        [Cell("Experience With", 3, CellType.TextView)]
        public string PetSitterEmployerAdditionalPetInformation { get; set; }

        [Cell("Sitter experience with animal required", 4, CellType.TextView)]
        public string PetSitterEmployerExpirienceRequired { get; set; }

        [Cell("Sitter experience with animal required", 5, CellType.TextView)]
        public string PetSitterEmployerExpirienceRequiredInformation { get; set; }

        [Cell("Additional Information", 6, CellType.TextView)]
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