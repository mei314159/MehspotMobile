using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mehspot.iOS.Views;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{

    public class SearchBabysitterFilterTableSource : SearchFilterTableSource
    {
        private SearchBabysitterDTO filter;

        private readonly BadgeService badgeService;

        public SearchBabysitterFilterTableSource (BadgeService badgeService, SearchBabysitterDTO filter) : base (filter)
        {
            this.filter = filter;
            this.badgeService = badgeService;
        }

        public override async Task InitializeAsync ()
        {
            var ageRanges = await GetAgeRangesAsync ();
            this.Cells.Add (SliderCell.Create (filter, a => a.Details.DistanceFrom, "Max Distance", 0, 200));
            this.Cells.Add (SliderCell.Create (filter, a => a.HourlyRate, "Max Hourly Rate ($)", 0, 200));
            var zipCell = TextEditCell.Create (filter.Details.ZipCode, a => filter.Details.ZipCode = a, "Zip");
            zipCell.Mask = "#####";
            this.Cells.Add (zipCell);

            this.Cells.Add (BooleanEditCell.Create (filter.OwnCar == true, v => filter.OwnCar = v == true ? v : (bool?)null, "Has Car"));
            this.Cells.Add (BooleanEditCell.Create (filter.Details.HasPicture == true, v => filter.Details.HasPicture = v == true ? v : (bool?)null, "Has Profile Picture"));
            this.Cells.Add (BooleanEditCell.Create (filter.Details.HasReferences == true, v => filter.Details.HasReferences = v == true ? v : (bool?)null, "Has References"));
            this.Cells.Add (BooleanEditCell.Create (filter.HasCertification == true, v => filter.HasCertification = v == true ? v : (bool?)null, "Has Certification"));
            this.Cells.Add (PickerCell.Create (filter.AgeRange, (property) => { filter.AgeRange = property; }, "Age Range", ageRanges));
        }

        private async Task<KeyValuePair<int?, string> []> GetAgeRangesAsync ()
        {
            var result = await badgeService.GetAgeRangesAsync (this.filter.BadgeId);
            if (result.IsSuccess) {
                return result.Data.Select (a => new KeyValuePair<int?, string> (a.Id, a.Name)).ToArray ();
            }

            return null;
        }
    }
}
