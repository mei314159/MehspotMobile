using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{
    [SearchFilterDto(Constants.BadgeNames.OtherJobs)]
    public class SearchOtherJobsDTO : SearchFilterDTOBase
    {
        public double? HourlyRate { get; set; }

        public string Gender { get; set; }

        [Cell(Label = "Jobs", CellType = CellType.Multiselect, Order = 0, OptionsKey = BadgeService.BadgeKeys.OtherJobsType)]
        public string[] Jobs { get; set; }

        public bool? IsHired { get; set; }

        [Cell(Label = "Age Range", CellType = CellType.Select, Order = 1, OptionsKey = BadgeService.BadgeKeys.OtherJobsAgeRange)]
        public string AgeRange { get; set; }
    }
}