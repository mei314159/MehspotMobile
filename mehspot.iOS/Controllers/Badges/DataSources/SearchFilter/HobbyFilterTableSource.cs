using System.Threading.Tasks;
using mehspot.iOS.Views;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using System.Linq;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{

    public class HobbyFilterTableSource : SearchFilterTableSource<SearchHobbyDTO>
    {
        public HobbyFilterTableSource (BadgeService badgeService, int badgeId) : base (badgeService, badgeId)
        {
        }

        public override async Task InitializeAsync ()
        {
            var genders = await GetOptionsAsync (BadgeService.BadgeKeys.Gender);
            var ageRanges = await GetOptionsAsync (BadgeService.BadgeKeys.HobbyAgeRange);
            var hobbyTypes = await GetOptionsAsync (BadgeService.BadgeKeys.HobbyType, true);
            this.Cells.Add (SliderCell.Create (TypedFilter, a => a.Details.DistanceFrom, "Max Distance", 0, 200));
            var zipCell = TextEditCell.Create (TypedFilter.Details.ZipCode, a => TypedFilter.Details.ZipCode = a, "Zip");
            zipCell.Mask = "#####";
            this.Cells.Add (zipCell);

            this.Cells.Add (PickerCell.CreateMultiselect<int?> (new int? [] { }, (property) => { TypedFilter.HobbyTypes = property?.Select (a => a.ToString ()).ToArray (); }, "Hobby", hobbyTypes));
            this.Cells.Add (PickerCell.Create<int?> (null, (property) => { TypedFilter.Gender = property?.ToString (); }, "Gender", genders));
            this.Cells.Add (PickerCell.Create<int?> (null, (property) => { TypedFilter.AgeRange = property?.ToString (); }, "Age Range", ageRanges));

            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasPicture == true, v => TypedFilter.Details.HasPicture = v == true ? v : (bool?)null, "Has Profile Picture"));
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasReferences == true, v => TypedFilter.Details.HasReferences = v == true ? v : (bool?)null, "Has References"));
        }
    }
    
}
