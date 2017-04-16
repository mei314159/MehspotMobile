namespace Mehspot.Core.DTO.Search
{

    public class SearchHobbyDTO : SearchFilterDTOBase
    {
        public string Gender { get; set; }

        public bool? IsTrained { get; set; }

        public string AgeRange { get; set; }

        public string[] HobbyTypes { get; set; }
    }
    
}