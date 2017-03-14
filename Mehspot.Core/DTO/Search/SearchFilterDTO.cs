namespace Mehspot.Core.DTO.Search
{
    public class SearchFilterDTO
    {
        public int? MaxDistance { get; set; }

        public string Zip { get; set; }

        public bool? HasPicture { get; set; }

        public bool? HasReferences { get; set; }
    }
    
}
