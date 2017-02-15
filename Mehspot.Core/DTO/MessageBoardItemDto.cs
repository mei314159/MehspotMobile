using System;
namespace Mehspot.Core.DTO
{
    public class MessageBoardItemDto
    {
        public ApplicationUserBaseDto WithUser { get; set; }

        public string ToUserId { get; set; }

        public DateTime SentDate { get; set; }

        public string LastMessage { get; set; }

        public bool IsRead { get; set; }

        public bool IsOnline { get; set; }
    }
}
