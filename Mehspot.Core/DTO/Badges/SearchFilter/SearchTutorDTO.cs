using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{
    [SearchFilterDto(Constants.BadgeNames.Tutor)]
    public class SearchTutorDTO : SearchFilterDTOBase
    {
        [Cell(Label = "Max Hourly Rate ($)", CellType = CellType.Range, Order = 0, MinValue = 0, MaxValue = 200)]
        public double? HourlyRate { get; set; }

        [Cell(Label = "Subjects", CellType = CellType.Multiselect, Order = 1, OptionsKey = BadgeService.BadgeKeys.TutorSubject)]
        public string[] Subjects { get; set; }

        [Cell(Label = "Can Travel", CellType = CellType.Boolean, Order = 2)]
        public bool? CanTravel { get; set; }
    }
}