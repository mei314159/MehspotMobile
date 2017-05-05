using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;

namespace Mehspot.Core.Contracts.ViewControllers
{

    public interface IMessagingViewController
    {
        string ToUserId { get; }

        string ToUserName { get; }

        string MessageFieldValue { get; set; }

        IViewHelper ViewHelper { get; }

        void DisplayMessages(Result<CollectionDto<MessageDto>> messagesResult);

        void ToggleMessagingControls(bool enabled);

        void AddMessageBubbleToEnd(MessageDto messageDto);
    }
}
