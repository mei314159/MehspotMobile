using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;

namespace Mehspot.Core.Contracts.ViewControllers
{
    public interface IUserProfileViewController
    {
        string UserName { get; set; }
        string FullName { get; set; }
        string ProfilePicturePath { get; set; }
        int RecommendationsCount { get; set; }
        int ReferencesCount { get; set; }

        IViewHelper ViewHelper { get; }

        void ReloadData();
    }
}
