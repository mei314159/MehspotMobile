namespace Mehspot.Core.DTO.Groups
{
    public class GroupPrememberDTO
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        public string EmailOrUsername { get; set; }

        public string Username { get; set; }

        public string UserId { get; set; }

        public string PhoneNumber { get; set; }

        public string Comment { get; set; }

        public bool IsExistingUser { get; set; }

        public bool AddedByEmail { get; set; }

        public GroupUserTypeEnum GroupUserType { get; set; }

        public string ProfilePicturePath { get; set; }
    }
}
