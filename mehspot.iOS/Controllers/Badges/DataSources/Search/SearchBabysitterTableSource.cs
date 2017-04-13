using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mehspot.iOS.Views;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{

    public class SearchBabysitterTableSource : SearchTableDataSource
    {
        private SearchBabysitterDTO filter;

        private readonly BadgeService badgeService;
        private readonly int BadgeId;

        public SearchBabysitterTableSource (BadgeService badgeService, int badgeId, SearchBabysitterDTO filter) : base (filter)
        {
            this.filter = filter;
            this.badgeService = badgeService;
            this.BadgeId = badgeId;
        }

        public override async Task InitializeAsync ()
        {
            var ageRanges = await GetAgeRangesAsync ();
            var cells = new List<UITableViewCell> ();
            cells.Add (SliderCell.Create (filter, a => a.Details.DistanceFrom, "Max Distance", 0, 200));
            cells.Add (SliderCell.Create (filter, a => a.HourlyRate, "Max Hourly Rate ($)", 0, 200));
            var zipCell = TextEditCell.Create (filter.Details.ZipCode, a => filter.Details.ZipCode = a, "Zip");
            zipCell.Mask = "#####";
            cells.Add (zipCell);

            cells.Add (BooleanEditCell.Create (filter.OwnCar == true, v => filter.OwnCar = v == true ? v : (bool?)null, "Has Car"));
            cells.Add (BooleanEditCell.Create (filter.Details.HasPicture == true, v => filter.Details.HasPicture = v == true ? v : (bool?)null, "Has Profile Picture"));
            cells.Add (BooleanEditCell.Create (filter.Details.HasReferences == true, v => filter.Details.HasReferences = v == true ? v : (bool?)null, "Has References"));
            cells.Add (BooleanEditCell.Create (filter.HasCertification == true, v => filter.HasCertification = v == true ? v : (bool?)null, "Has Certification"));
            cells.Add (PickerCell.Create (filter.AgeRange, (property) => { filter.AgeRange = property; }, "Age Range", ageRanges));

            this.Cells.AddRange (cells);
        }

        private async Task<KeyValuePair<int?, string> []> GetAgeRangesAsync ()
        {
            var result = await badgeService.GetAgeRangesAsync (BadgeId);
            if (result.IsSuccess) {
                return result.Data.Select (a => new KeyValuePair<int?, string> (a.Id, a.Name)).ToArray ();
            }

            return null;
        }
    }
}
