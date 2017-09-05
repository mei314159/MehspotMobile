using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{
    [SearchFilterDto(Constants.BadgeNames.Babysitter)]
    public class SearchBabysitterDTO : ISearchQueryDTO
    {
        public int BadgeId { get; set; }

        [Cell(Label = "Max Hourly Rate ($)", CellType = CellType.Range, Order = 0, MinValue = 0, MaxValue = 50)]
        public int? HourlyRate { get; set; }

        [Cell(Label = "Has Car", CellType = CellType.Boolean, Order = 1)]
        public bool? OwnCar { get; set; }

        [Cell(Label = "Has Certifications", CellType = CellType.Boolean, Order = 2)]
        public bool? HasCertification { get; set; }

        [Cell(Label = "Age Range", CellType = CellType.Select, Order = 3, OptionsKey = BadgeService.BadgeKeys.AgeRange)]
        public int? AgeRange { get; set; }
        IBaseFilterDTO details;

        [Cell(CellType = CellType.Complex, Order = 0)]
        public IBaseFilterDTO Details
        {
            get
            {
                if (details == null){
                    details = new BabysitterFilterDTO();
                }
                return details;
            }

            set
            {
                details = value;
            }
        }
    }
}
