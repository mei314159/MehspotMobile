namespace Mehspot.Core.DTO.Search
{

    public class SearchKidsPlayDateDTO : SearchFilterDTOBase
    {
        public string Gender { get; set; }

        public bool? IsPlayed { get; set; }

        public int? Age { get; set; }

        public int? MaxAge { get; set; }
    }
}