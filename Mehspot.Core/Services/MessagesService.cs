using System;
using System.Net.Http;
using mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using System.Collections.Generic;
using mehspot.Core;

namespace Mehspot.Core.Services
{
    public class MessagesService : BaseDataService
    {
        public MessagesService (IApplicationDataStorage applicationDataStorage) : base (applicationDataStorage)
        {
        }

        public Action<int, object> OnSendNotification;

        public Task<Result<MessageBoardItemDto []>> GetMessageBoard (string filter)
        {
            return GetAsync<MessageBoardItemDto[]>("Badges/MessageBoard?filter=" + filter);
        }

        public Task<Result<CollectionDto<MessageDto>>> GetMessages (int pageNumber, string toUserId)
        {
            return GetAsync<CollectionDto<MessageDto>>($"Badges/GetMessages?toUserId={toUserId}&pageNumber={pageNumber}");
        }

        public Task<Result<MessageDto>> SendMessageAsync (string message, string toUserId)
        {
            var data = new Dictionary<string, string> ();
            data.Add ("Message", message);
            data.Add ("ToUserId", toUserId);
            data.Add ("FromUserId", this.ApplicationDataStorage.AuthInfo.UserId);

            return SendDataAsync<MessageDto> ("Badges/Send", HttpMethod.Post, new FormUrlEncodedContent (data));

        }


        public async Task<Result> MarkMessagesReadAsync (string toUserId)
        {
            var data = new FormUrlEncodedContent (new Dictionary<string, string> {
                { "toUserId", toUserId }
            });

            return await SendDataAsync<object> ("Badges/MarkMessagesRead", HttpMethod.Post, data).ConfigureAwait (false);
        }
    }
}