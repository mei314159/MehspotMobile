using Mehspot.Core.Contracts.Wrappers;

namespace Mehspot.Core.Contracts.ViewControllers
{
    public interface IEditBadgeProfileController
    {
        IViewHelper ViewHelper { get; }
        string BadgeName { get; }
        int BadgeId { get; }
        bool BadgeIsRegistered { get; set; }
        bool RedirectToSearchResults { get; }
        bool SaveButtonEnabled { get; set; }
        string WindowTitle { get; set; }
        void ReloadData();
        void Dismiss();
        void HideKeyboard();
        void GoToSearchResults();
    }
}