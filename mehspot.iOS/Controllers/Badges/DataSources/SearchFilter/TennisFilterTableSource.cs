using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mehspot.iOS.Views;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{

    public class TennisFilterTableSource : SearchFilterTableSource<SearchTennisDTO>
    {
        readonly BadgeService badgeService;

        public TennisFilterTableSource (BadgeService badgeService, int badgeId) : base (badgeService, badgeId)
        {
            this.badgeService = badgeService;
        }

        public override async Task InitializeAsync ()
        {
            var ageRanges = await GetAgeRangesAsync ();
            var genders = await GetGendersAsync ();
            var skillLevels = await GetSkillLevelsAsync ();

            this.Cells.Add (SliderCell.Create (TypedFilter, a => a.Details.DistanceFrom, "Max Distance", 0, 200));
            var zipCell = TextEditCell.Create (TypedFilter.Details.ZipCode, a => TypedFilter.Details.ZipCode = a, "Zip");
            zipCell.Mask = "#####";
            this.Cells.Add (zipCell);

            this.Cells.Add (PickerCell.Create (TypedFilter.Gender, (property) => { TypedFilter.Gender = property; }, "Gender", genders));
            this.Cells.Add (PickerCell.Create (TypedFilter.AgeRange, (property) => { TypedFilter.AgeRange = property; }, "Age Range", ageRanges));
            this.Cells.Add (PickerCell.Create (TypedFilter.SkillLevel, (property) => { TypedFilter.SkillLevel = property; }, "Skill Level", skillLevels));

            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasPicture == true, v => TypedFilter.Details.HasPicture = v == true ? v : (bool?)null, "Has Profile Picture"));
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasReferences == true, v => TypedFilter.Details.HasReferences = v == true ? v : (bool?)null, "Has References"));
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.HasCourt == true, v => TypedFilter.HasCourt = v == true ? v : (bool?)null, "Has Court"));

        }

        protected async Task<KeyValuePair<string, string> []> GetGendersAsync ()
        {
            var result = await badgeService.GetBadgeKeysAsync (this.TypedFilter.BadgeId, BadgeService.BadgeKeys.Gender);
            if (result.IsSuccess) {
                return result.Data.Select (a => new KeyValuePair<string, string> (a.Id.ToString(), a.Name)).ToArray ();
            }

            return null;
        }

        protected async Task<KeyValuePair<string, string> []> GetSkillLevelsAsync ()
        {
            var result = await badgeService.GetBadgeKeysAsync (this.TypedFilter.BadgeId, BadgeService.BadgeKeys.SkillLevel);
            if (result.IsSuccess) {
                return result.Data.Select (a => new KeyValuePair<string, string> (a.Id.ToString(), a.Name)).ToArray ();
            }

            return null;
        }
    }
}
