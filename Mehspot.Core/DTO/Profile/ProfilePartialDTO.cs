namespace Mehspot.Core.DTO.Profile
{

    public class ProfilePartialDTO
    {
        public string Id { get; set; }
        public int? State { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public int? SubdivisionId { get; set; }
        public int? SubdivisionOptionId { get; set; }
    }
}