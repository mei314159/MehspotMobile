namespace Mehspot.Core.DTO.Search
{
    public interface ISearchFilterDTO
    {
        int BadgeId { get; set; }
        BaseFilterDTO Details { get; set; }
    }

    public class SearchFilterDTOBase : ISearchFilterDTO
    {
        public int BadgeId { get; set; }
        public BaseFilterDTO Details { get; set; } = new BaseFilterDTO();
    }
}
