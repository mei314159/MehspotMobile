using System;
using System.Linq;
using System.Threading.Tasks;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.DTO;
using Mehspot.Core.Messaging;
using Mehspot.Core.Services;

namespace Mehspot.Core.Models
{

    public class MessageBoardModel
    {
        private volatile bool loading;

        private readonly MessagesService messagesService;
        private readonly IMessageBoardViewController viewController;

        public MessageBoardItemDto[] Items;
        public event Action LoadingStart;
        public event Action<Result<MessageBoardItemDto[]>> LoadingEnd;
        public volatile bool dataLoaded;

        public MessageBoardModel(MessagesService messagesService, IMessageBoardViewController viewController)
        {
            this.viewController = viewController;
            this.messagesService = messagesService;
            MehspotAppContext.Instance.ReceivedNotification += OnSendNotification;
        }

        public int Page { get; private set; } = 1;

        public async Task LoadMessageBoardAsync(bool isFirstLoad = false)
        {
            if (loading)
                return;
            loading = true;

            if (isFirstLoad)
            {
                LoadingStart?.Invoke();
            }

            var result = await messagesService.GetMessageBoard(viewController.Filter);
            if (result.IsSuccess)
            {
                this.Items = result.Data;
            }

            LoadingEnd?.Invoke(result);
            dataLoaded = result.IsSuccess;
            loading = false;
        }

        void OnSendNotification(MessagingNotificationType notificationType, MessageDto data)
        {
            if (notificationType == MessagingNotificationType.Message && Items != null)
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    var item = Items[i];
                    if (item.WithUser.Id == data.FromUserId)
                    {
                        item.UnreadMessagesCount++;
                        viewController.UpdateMessageBoardCell(item, i);
                        break;
                    }
                }
            }
        }
    }
}
