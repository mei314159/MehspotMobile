using System.Collections.Generic;

namespace MehSpot.Core.DTO.Subdivision
{
    public class SubdivisionOptionDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int AddressId { get; set; }

        public int VerificationCount => VerifiedByUserIds.Count;

        public int SubdivisionId { get; set; }

        public virtual AddressDTO Address { get; set; }

        public virtual ICollection<string> VerifiedByUserIds { get; set; } = new List<string>();
    }
}
