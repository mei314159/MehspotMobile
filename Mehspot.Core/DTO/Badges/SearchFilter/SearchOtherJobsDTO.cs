namespace Mehspot.Core.DTO.Search
{

    public class SearchOtherJobsDTO : SearchFilterDTOBase
    {
        public double? HourlyRate { get; set; }

        public string Gender { get; set; }

        public string[] Jobs { get; set; }

        public bool? IsHired { get; set; }

        public string AgeRange { get; set; }
    }
}