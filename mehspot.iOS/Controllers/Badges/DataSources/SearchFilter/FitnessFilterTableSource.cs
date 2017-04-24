using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mehspot.iOS.Views;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{
    public class FitnessFilterTableSource : SearchFilterTableSource<SearchFitnessDTO>
    {
        public FitnessFilterTableSource (BadgeService badgeService, int badgeId) : base (badgeService, badgeId)
        {
        }

        public override async Task InitializeAsync ()
        {
            var fitnessTypes = await GetOptionsAsync (BadgeService.BadgeKeys.FitnessType, true);
            var ageRanges = await GetOptionsAsync (BadgeService.BadgeKeys.FitnessAgeRange);
            var genders = await GetOptionsAsync (BadgeService.BadgeKeys.Gender);
            this.Cells.Add (SliderCell.Create<int?>(20, a => TypedFilter.Details.DistanceFrom = a, "Max Distance", 0, 200));
            var zipCell = TextEditCell.Create (TypedFilter.Details.ZipCode, a => TypedFilter.Details.ZipCode = a, "Zip");
            zipCell.Mask = "#####";
            this.Cells.Add (zipCell);

            this.Cells.Add (PickerCell.CreateMultiselect<int?> (new int? [] { }, (property) => { TypedFilter.FitnessTypes = property?.Select (a => a.ToString ()).ToArray (); }, "Fitness Type", fitnessTypes));
            this.Cells.Add (PickerCell.Create ((int?)null, (property) => { TypedFilter.Gender = property?.ToString (); }, "Gender", genders));
            this.Cells.Add (PickerCell.Create ((int?)null, (property) => { TypedFilter.AgeRange = property?.ToString (); }, "Age Range", ageRanges));

            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasPicture == true, v => TypedFilter.Details.HasPicture = v == true ? v : (bool?)null, "Has Profile Picture"));
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasReferences == true, v => TypedFilter.Details.HasReferences = v == true ? v : (bool?)null, "Has References"));
        }
    }
}
