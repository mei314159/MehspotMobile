namespace Mehspot.Core.DTO.Search
{

    public class SearchTennisDTO : SearchFilterDTOBase
    {
        public bool? HasCourt { get; set; }

        public string Gender { get; set; }

        public string SkillLevel { get; set; }

        public string AgeRange { get; set; }
    }

    
}
