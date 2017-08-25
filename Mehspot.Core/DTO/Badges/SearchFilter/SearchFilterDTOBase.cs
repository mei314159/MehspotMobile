namespace Mehspot.Core.DTO.Search
{
    public class SearchFilterDTOBase : ISearchQueryDTO
    {
        public int BadgeId { get; set; }

        [Cell(CellType = CellType.Complex, Order = 0)]
        public virtual IBaseFilterDTO Details { get; set; } = new BaseFilterDTO();
    }
}
