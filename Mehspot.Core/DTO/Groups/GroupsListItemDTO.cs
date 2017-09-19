using System;
namespace Mehspot.Core.DTO.Groups
{
    public class GroupsListItemDTO
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public int? MessageId { get; set; }

        public DateTime? SentDate { get; set; }

        public string LastMessage { get; set; }

        public string LastMessageUserId { get; set; }

        public GroupTypeEnum GroupType { get; set; }

        public GroupUserTypeEnum GroupUserType { get; set; }

        public DateTime? LastSeen { get; set; }

        public bool HasUnreadMessages { get; set; }
    }
}
