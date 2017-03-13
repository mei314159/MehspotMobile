using System;
namespace Mehspot.Core.DTO
{
    public class MessageDto
    {
        public int Id { get; set; }

        public string FromUserId { get; set; }

        public string ToUserId { get; set; }

        public string ToUserName { get; set; }

        public DateTime SendDate { get; set; }

        public bool IsRead { get; set; }

        public string Message { get; set; }
    }
}
