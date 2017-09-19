using System;
using System.Collections;
using System.Collections.Generic;
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

        public readonly List<MessageBoardItemDto> Items = new List<MessageBoardItemDto>();
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
                lock (((ICollection)this.Items).SyncRoot)
                {
                    this.Items.Clear();
                    this.Items.AddRange(result.Data.OrderByDescending(a => a.UnreadMessagesCount)
                                        .ThenByDescending(a => a.SentDate));
                }
            }

            LoadingEnd?.Invoke(result);
            dataLoaded = result.IsSuccess;
            loading = false;
        }

        void OnSendNotification(MessagingNotificationType notificationType, MessageDto data)
        {
            if (notificationType == MessagingNotificationType.Message && Items != null)
            {
                UpdateList(data);
            }
        }

        public void UpdateList(MessageDto data)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (item.WithUser.Id == data.FromUserId)
                {
                    item.UnreadMessagesCount++;
                    item.LastMessage = data.Message;
                    item.SentDate = data.SentDate;
                    item.IsRead = data.IsRead;
                    item.ToUserId = data.ToUserId;
                    Items.Remove(item);
                    Items.Insert(0, item);
                    viewController.DisplayMessageBoard();
                    break;
                }
            }
        }
    }
}



