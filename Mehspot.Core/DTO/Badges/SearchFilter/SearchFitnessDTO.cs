namespace Mehspot.Core.DTO.Search
{

    public class SearchFitnessDTO : SearchFilterDTOBase
    {
        public string Gender { get; set; }

        public string AgeRange { get; set; }

        public string[] FitnessTypes { get; set; }
    }
}