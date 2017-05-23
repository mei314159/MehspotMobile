namespace Mehspot.Core.DTO.Search
{
    public interface ISearchQueryDTO
    {
        int BadgeId { get; set; }
        BaseFilterDTO Details { get; set; }
    }

    public class SearchFilterDTOBase : ISearchQueryDTO
    {
        public int BadgeId { get; set; }

        [Cell(CellType = CellType.Complex, Order = 0)]
        public BaseFilterDTO Details { get; set; } = new BaseFilterDTO();
    }
}
