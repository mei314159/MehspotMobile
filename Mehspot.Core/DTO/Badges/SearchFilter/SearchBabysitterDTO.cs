using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{
    public class SearchBabysitterDTO : SearchFilterDTOBase
    {
        [Cell(Label = "Max Hourly Rate ($)", CellType = CellType.Range, Order = 0, MinValue = 0, MaxValue = 200)]
        public int? HourlyRate { get; set; }

        [Cell(Label = "Has Car", CellType = CellType.Boolean, Order = 1)]
        public bool? OwnCar { get; set; }

        [Cell(Label = "Has Certifications", CellType = CellType.Boolean, Order = 2)]
        public bool? HasCertification { get; set; }

        [Cell(Label = "Age Range", CellType = CellType.Select, Order = 3, OptionsKey = BadgeService.BadgeKeys.AgeRange)]
        public int? AgeRange { get; set; }
    }
}
