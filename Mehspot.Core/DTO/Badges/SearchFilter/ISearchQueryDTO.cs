namespace Mehspot.Core.DTO.Search
{
    public interface ISearchQueryDTO
    {
        int BadgeId { get; set; }

        IBaseFilterDTO Details { get; }
    }
}
