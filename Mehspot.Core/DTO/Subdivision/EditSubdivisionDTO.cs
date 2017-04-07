namespace MehSpot.Core.DTO.Subdivision
{
    public class EditSubdivisionDTO
    {
        public int Id { get; set; }

        public string SubdivisionIdentifier { get; set; }

        public string Name { get; set; }

        public int AddressId { get; set; }

        public string ZipCode { get; set; }

        public virtual AddressDTO Address { get; set; }
    }
}