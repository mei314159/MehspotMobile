using Mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using Mehspot.Core.DTO.Groups;
using System;
using System.Net.Http;
using System.Collections.Generic;

namespace Mehspot.Core.Services
{
    public class GroupService : BaseDataService
    {
        public GroupService(IApplicationDataStorage applicationDataStorage) : base(applicationDataStorage)
        {
        }

        public Task<Result<GroupsListItemDTO[]>> GetList()
        {
            return GetAsync<GroupsListItemDTO[]>("groups/list");
        }

        public Task<Result<GroupPrememberDTO[]>> GetMembersAsync(int groupId)
        {
            return GetAsync<GroupPrememberDTO[]>($"groups/{groupId}/members");
        }

        public Task<Result<GroupMessageDTO[]>> GetMessages(int pageNumber, int groupId)
        {
            return GetAsync<GroupMessageDTO[]>($"groups/{groupId}/messages?pageNumber={pageNumber}");
        }

        public Task<Result<GroupMessageDTO>> SendMessageAsync(string message, int groupId)
        {
            var data = new Dictionary<string, string>();
            data.Add("Message", message);

            return SendDataAsync<GroupMessageDTO>($"groups/{groupId}/messages", HttpMethod.Post, new FormUrlEncodedContent(data));

        }

        public async Task<Result> LeaveGroupAsync(int groupId)
        {
            var result = await this.PostAsync<object>($"groups/{groupId}/leave", null).ConfigureAwait(false);
            return result;
        }
    }
}
