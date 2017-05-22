using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{
    public class SearchTutorDTO : SearchFilterDTOBase
    {
        [SearchProperty(Label = "Max Hourly Rate ($)", CellType = CellType.Range, Order = 0, MinValue = 0, MaxValue = 200)]
        public double? HourlyRate { get; set; }

        [SearchProperty(Label = "Subjects", CellType = CellType.Multiselect, Order = 1, OptionsKey = BadgeService.BadgeKeys.TutorSubject)]
        public string[] Subjects { get; set; }

        [SearchProperty(Label = "Can Travel", CellType = CellType.Boolean, Order = 2)]
        public int? CanTravel { get; set; }
    }
}