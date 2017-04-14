namespace Mehspot.Core.DTO.Search
{
    public interface ISearchFilterDTO
    {
        int BadgeId { get; set; }
        BaseFilterDTO Details { get; set; }
    }

}
