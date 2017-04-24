using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mehspot.iOS.Views;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{

    public class GolfFilterTableSource : SearchFilterTableSource<SearchGolfDTO>
    {
        readonly BadgeService badgeService;

        public GolfFilterTableSource (BadgeService badgeService, int badgeId) : base (badgeService, badgeId)
        {
            this.badgeService = badgeService;
        }

        public override async Task InitializeAsync ()
        {
            var ageGroups = await GetGolfAgeGroupsAsync ();
            var genders = await GetGendersAsync ();

            this.Cells.Add (SliderCell.Create<int?> (20, a => TypedFilter.Details.DistanceFrom = a, "Max Distance", 0, 200));
            var zipCell = TextEditCell.Create (TypedFilter.Details.ZipCode, a => TypedFilter.Details.ZipCode = a, "Zip");
            zipCell.Mask = "#####";
            this.Cells.Add (zipCell);

            this.Cells.Add (SliderCell.Create<double?> (null, (a) => TypedFilter.Handicap = a, "Min Handicap", 0, 100));
            this.Cells.Add (SliderCell.Create<double?> (null, a => TypedFilter.MaxHandicap = a, "Max Handicap", 0, 100));
            this.Cells.Add (PickerCell.Create (TypedFilter.Gender, (property) => { TypedFilter.Gender = property; }, "Gender", genders));
            this.Cells.Add (PickerCell.Create (TypedFilter.AgeRange, (property) => { TypedFilter.AgeRange = property; }, "Age Group", ageGroups));

            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasPicture == true, v => TypedFilter.Details.HasPicture = v == true ? v : (bool?)null, "Has Profile Picture"));
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasReferences == true, v => TypedFilter.Details.HasReferences = v == true ? v : (bool?)null, "Has References"));
        }

        protected async Task<KeyValuePair<string, string> []> GetGendersAsync ()
        {
            var result = await badgeService.GetBadgeKeysAsync (this.TypedFilter.BadgeId, BadgeService.BadgeKeys.Gender);
            if (result.IsSuccess) {
                return result.Data.Select (a => new KeyValuePair<string, string> (a.Id.ToString (), a.Name)).ToArray ();
            }

            return null;
        }

        protected async Task<KeyValuePair<string, string> []> GetGolfAgeGroupsAsync ()
        {
            var result = await badgeService.GetBadgeKeysAsync (this.TypedFilter.BadgeId, BadgeService.BadgeKeys.GolfAgeGroup);
            if (result.IsSuccess) {
                return result.Data.Select (a => new KeyValuePair<string, string> (a.Id.ToString (), a.Name)).ToArray ();
            }

            return null;
        }
    }
}
