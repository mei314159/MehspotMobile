using System;

namespace Mehspot.Core.DTO
{

    public class UserProfileDto : ValidationModelBase
    {
        public new string Id { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public int? State { get; set; }

        public string Zip { get; set; }

        public string SubdivisionName { get; set; }

        public int? SubdivisionId { get; set; }

        public int? SubdivisionOptionId { get; set; }

        public string ProfilePicturePath { get; set; }

        public bool MehspotNotificationsEnabled { get; set; }

        public bool AllGroupsNotificationsEnabled { get; set; }
    }

    
}
