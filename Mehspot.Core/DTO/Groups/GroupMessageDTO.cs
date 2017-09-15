using System;
namespace Mehspot.Core.DTO.Groups
{
    public class GroupMessageDTO
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public int MessageId { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }

        public string ProfilePicturePath { get; set; }

        public bool Liked { get; set; }

        public DateTime Posted { get; set; }
    }
}
