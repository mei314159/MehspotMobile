using Mehspot.Core.DTO.Badges;

namespace Mehspot.Models.ViewModels
{
    public interface ISearchResultDTO: IAdditionalInfo
    {
        BadgeUserDetailsFilterDTO Details { get; set; }
    }
}