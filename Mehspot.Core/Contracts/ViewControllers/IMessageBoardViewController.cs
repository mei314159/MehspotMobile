using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;

namespace Mehspot.Core.Contracts.ViewControllers
{

    public interface IMessageBoardViewController
    {
        string Filter { get; }

        IViewHelper ViewHelper { get; }

        void DisplayMessageBoard();

        void UpdateApplicationBadge(int value);

        void UpdateMessageBoardCell(MessageBoardItemDto dto, int index);
    }
}
