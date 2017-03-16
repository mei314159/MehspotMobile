namespace Mehspot.Core.DTO.Search
{
    public class BaseFilterDTO
    {
        public int? DistanceFrom { get; set; }

        public string ZipCode { get; set; }

        public bool? HasPicture { get; set; }

        public bool? HasReferences { get; set; }
    }
    
}
