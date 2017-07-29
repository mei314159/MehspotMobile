using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Models.ViewModels;

namespace Mehspot.Core
{

    public interface IViewBadgeProfileController
    {
        int BadgeId { get; }
        string BadgeName { get; }
        string UserId { get; }
        IViewHelper ViewHelper { get; }

        string WindowTitle { get; set; }
        string Subdivision { get; set; }
        string Distance { get; set; }
        string Likes { get; set; }
        string FirstName { get; set; }
        string InfoLabel1 { get; set; }
        string InfoLabel2 { get; set; }
        bool HideFavoriteIcon { get; set; }
        bool EnableSendMessageButton { get; set; }
        void SetProfilePictureUrl(string value);

        void ReloadCells();
    }
}
