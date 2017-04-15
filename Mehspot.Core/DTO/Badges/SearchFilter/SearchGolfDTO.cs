namespace Mehspot.Core.DTO.Search
{

    public class SearchGolfDTO : SearchFilterDTOBase
    {
        public double? Handicap { get; set; }

        public double? MaxHandicap { get; set; }

        public string Gender { get; set; }

        public string AgeRange { get; set; }
    }
}
