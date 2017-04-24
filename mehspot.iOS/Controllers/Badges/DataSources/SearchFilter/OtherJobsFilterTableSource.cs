using System.Threading.Tasks;
using mehspot.iOS.Views;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using System.Linq;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{

    public class OtherJobsFilterTableSource : SearchFilterTableSource<SearchOtherJobsDTO>
    {
        public OtherJobsFilterTableSource (BadgeService badgeService, int badgeId) : base (badgeService, badgeId)
        {
        }

        public override async Task InitializeAsync ()
        {
            var ageRanges = await GetOptionsAsync (BadgeService.BadgeKeys.OtherJobsAgeRange);
            var jobs = await GetOptionsAsync (BadgeService.BadgeKeys.OtherJobsType, true);
            this.Cells.Add (SliderCell.Create<int?>  (20, a => TypedFilter.Details.DistanceFrom = a, "Max Distance", 0, 200));
            var zipCell = TextEditCell.Create (TypedFilter.Details.ZipCode, a => TypedFilter.Details.ZipCode = a, "Zip");
            zipCell.Mask = "#####";
            this.Cells.Add (zipCell);

            this.Cells.Add (PickerCell.CreateMultiselect<int?> (new int? [] { }, (property) => { TypedFilter.Jobs = property?.Select (a => a.ToString ()).ToArray (); }, "Hobby", jobs));
            this.Cells.Add (PickerCell.Create<int?> (null, (property) => { TypedFilter.AgeRange = property?.ToString (); }, "Age Range", ageRanges));

            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasPicture == true, v => TypedFilter.Details.HasPicture = v == true ? v : (bool?)null, "Has Profile Picture"));
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasReferences == true, v => TypedFilter.Details.HasReferences = v == true ? v : (bool?)null, "Has References"));
        }
    }
}
