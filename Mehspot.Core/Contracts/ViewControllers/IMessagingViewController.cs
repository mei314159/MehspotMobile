using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;

namespace Mehspot.Core.Contracts.ViewControllers
{

    public interface IMessagingViewController
    {
        string ToUserId { get; }
        string ToUserName { get; }
        string MessageFieldValue { get; set; }
        string ProfilePicturePath { get; set; }

        IViewHelper ViewHelper { get; }

        void ScrollToEnd();

        void ScrollingDown();
        void DisplayMessages(Result<CollectionDto<MessageDto>> messagesResult);
        void ToggleMessagingControls(bool enabled);
        void AddMessageBubbleToEnd(MessageDto messageDto);
    }
}
