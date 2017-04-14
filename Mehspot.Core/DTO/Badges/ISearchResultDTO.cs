using Mehspot.Core.DTO.Badges;

namespace MehSpot.Models.ViewModels
{
    public interface ISearchResultDTO: IAdditionalInfo
    {
        BadgeUserDetailsFilterDTO Details { get; set; }
    }
}