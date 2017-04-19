using System.Threading.Tasks;
using mehspot.iOS.Views;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{

    public class TennisFilterTableSource : SearchFilterTableSource<SearchTennisDTO>
    {
        public TennisFilterTableSource (BadgeService badgeService, int badgeId) : base (badgeService, badgeId)
        {
        }

        public override async Task InitializeAsync ()
        {
            var ageRanges = await GetOptionsAsync (BadgeService.BadgeKeys.TennisAgeRange);
            var genders = await GetOptionsAsync (BadgeService.BadgeKeys.Gender);
            var skillLevels = await GetOptionsAsync (BadgeService.BadgeKeys.SkillLevel);

            this.Cells.Add (SliderCell.Create (TypedFilter, a => a.Details.DistanceFrom, "Max Distance", 0, 200));
            var zipCell = TextEditCell.Create (TypedFilter.Details.ZipCode, a => TypedFilter.Details.ZipCode = a, "Zip");
            zipCell.Mask = "#####";
            this.Cells.Add (zipCell);

            this.Cells.Add (PickerCell.Create ((int?)null, (property) => { TypedFilter.Gender = property?.ToString (); }, "Gender", genders));
            this.Cells.Add (PickerCell.Create ((int?)null, (property) => { TypedFilter.AgeRange = property?.ToString (); }, "Age Range", ageRanges));
            this.Cells.Add (PickerCell.Create ((int?)null, (property) => { TypedFilter.SkillLevel = property?.ToString (); }, "Skill Level", skillLevels));

            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasPicture == true, v => TypedFilter.Details.HasPicture = v == true ? v : (bool?)null, "Has Profile Picture"));
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasReferences == true, v => TypedFilter.Details.HasReferences = v == true ? v : (bool?)null, "Has References"));
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.HasCourt == true, v => TypedFilter.HasCourt = v == true ? v : (bool?)null, "Has Court"));
        }
    }
}
