using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Groups;

namespace Mehspot.Core.Contracts.ViewControllers
{
    public interface IGroupMessagingViewController
    {
        int GroupId { get; }
        string GroupName { get; }
        GroupTypeEnum GroupType { get; set; }
        GroupUserTypeEnum GroupUserType { get; set; }

        string MessageFieldValue { get; set; }

        IViewHelper ViewHelper { get; }

        void ScrollToEnd();
        void DisplayMessages(Result<GroupMessageDTO[]> messagesResult);
        void ToggleMessagingControls(bool enabled);
        void AddMessageBubbleToEnd(GroupMessageDTO messageDto);
    }
}
