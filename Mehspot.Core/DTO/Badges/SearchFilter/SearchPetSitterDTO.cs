namespace Mehspot.Core.DTO.Search
{

    public class SearchPetSitterDTO : SearchFilterDTOBase
    {
        public double? HourlyRate { get; set; }

        public string Gender { get; set; }

        public string[] PetType { get; set; }

        public bool? CanTravel { get; set; }

        public bool IsHired { get; set; }
    }
    
}