using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO.Groups;

namespace Mehspot.Core.Contracts.ViewControllers
{
    public interface IGroupsListViewController
    {
        IViewHelper ViewHelper { get; }
        void DisplayGroups();
    }
}
