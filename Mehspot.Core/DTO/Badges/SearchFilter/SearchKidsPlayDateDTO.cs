namespace Mehspot.Core.DTO.Search
{

    public class SearchKidsPlayDateDTO : SearchFilterDTOBase
    {

        public string Gender { get; set; }

        public bool? IsPlayed { get; set; }

        [Cell(Label = "Min Age", CellType = CellType.Range, Order = 0, MinValue = 1, MaxValue = 21, DefaultValue = 1)]
        public int? Age { get; set; }

        [Cell(Label = "Max Age", CellType = CellType.Range, Order = 1, MinValue = 1, MaxValue = 21, DefaultValue = 21)]
        public int? MaxAge { get; set; }
    }
}