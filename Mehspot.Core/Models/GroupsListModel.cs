using System;
using System.Linq;
using System.Threading.Tasks;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Groups;
using Mehspot.Core.Messaging;
using Mehspot.Core.Services;

namespace Mehspot.Core.Models
{

    public class GroupsListModel
    {
        private volatile bool loading;

        private readonly GroupService groupsService;
        private readonly IGroupsListViewController viewController;

        public GroupsListItemDTO[] Items;
        public event Action LoadingStart;
        public event Action<Result<GroupsListItemDTO[]>> LoadingEnd;
        public volatile bool dataLoaded;

        public GroupsListModel(GroupService groupsService, IGroupsListViewController viewController)
        {
            this.viewController = viewController;
			this.groupsService = groupsService;
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

            var result = await groupsService.GetList().ConfigureAwait(false);
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
            //if (notificationType == MessagingNotificationType.Message && Items != null)
            //{
            //    for (int i = 0; i < Items.Length; i++)
            //    {
            //        var item = Items[i];
            //        if (item.WithUser.Id == data.FromUserId)
            //        {
            //            item.UnreadMessagesCount++;
            //            viewController.UpdateCell(item, i);
            //            break;
            //        }
            //    }
            //}
        }
    }
}
