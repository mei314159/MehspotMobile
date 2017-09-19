using System;
using System.Collections;
using System.Collections.Generic;
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

        public List<GroupsListItemDTO> Items = new List<GroupsListItemDTO>();
        public event Action LoadingStart;
        public event Action<Result<GroupsListItemDTO[]>> LoadingEnd;

        public GroupsListModel(GroupService groupsService, IGroupsListViewController viewController)
        {
            this.viewController = viewController;
            this.groupsService = groupsService;
            MehspotAppContext.Instance.ReceivedGroupNotification += OnSendNotification;
        }

        public int Page { get; private set; } = 1;

        public async Task LoadGroupsListAsync(bool isFirstLoad = false)
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
                lock (((ICollection)this.Items).SyncRoot)
                {
                    this.Items.Clear();
                    this.Items.AddRange(result.Data.OrderByDescending(a => a.HasUnreadMessages).ThenByDescending(a => a.SentDate));
                }
            }

            LoadingEnd?.Invoke(result);
            loading = false;
        }

        void OnSendNotification(MessagingNotificationType notificationType, GroupMessageDTO data)
        {
            if (notificationType == MessagingNotificationType.GroupMessage && Items != null)
            {
                UpdateList(data);
            }
        }

        public void UpdateList(GroupMessageDTO data)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (item.GroupId == data.GroupId)
                {
                    if (item.LastMessageUserId != MehspotAppContext.Instance.AuthManager.AuthInfo.UserId)
                    {
                        item.HasUnreadMessages = true;
                    }

                    item.LastMessage = data.Message;
                    item.MessageId = data.MessageId;
                    item.LastMessageUserId = data.UserId;
                    item.SentDate = data.Posted;
                    Items.Remove(item);
                    Items.Insert(0, item);
                    viewController.DisplayGroups();
                    break;
                }
            }
        }
    }
}
