namespace Mehspot.Core.DTO.Search
{
    [SearchFilterDto(Constants.BadgeNames.KidsPlayDate)]
    public class SearchKidsPlayDateDTO : SearchFilterDTOBase
    {
        public string Gender { get; set; }

        public bool? IsPlayed { get; set; }

        [Cell(Label = "Age", CellType = CellType.Range, Order = 0, MinValue = 1, MaxValue = 21, DefaultValue = 1, MaxValueProperty = nameof(MaxAge))]
        public int? Age { get; set; }

        [Cell(Label = "Max Age", CellType = CellType.Range, Order = 1, MinValue = 1, MaxValue = 21, DefaultValue = 21, MinValueProperty = nameof(Age))]
        public int? MaxAge { get; set; }
    }
}