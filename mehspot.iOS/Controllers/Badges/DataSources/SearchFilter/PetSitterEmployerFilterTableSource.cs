using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mehspot.iOS.Views;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{

    public class PetSitterEmployerFilterTableSource : SearchFilterTableSource<SearchPetSitterEmployerDTO>
    {
        public PetSitterEmployerFilterTableSource (BadgeService badgeService, int badgeId) : base (badgeService, badgeId)
        {
        }

        public override async Task InitializeAsync ()
        {
            var petTypes = await GetOptionsAsync (BadgeService.BadgeKeys.PetSitterEmployerPetType, true);
            this.Cells.Add (SliderCell.Create<int?> (20, a => TypedFilter.Details.DistanceFrom = a, "Max Distance", 0, 200));
            var zipCell = TextEditCell.Create (TypedFilter.Details.ZipCode, a => TypedFilter.Details.ZipCode = a, "Zip");
            zipCell.Mask = "#####";
            this.Cells.Add (zipCell);

            this.Cells.Add (PickerCell.CreateMultiselect<int?> (new int? [] { }, (property) => { TypedFilter.PetType = property?.Select (a => a.ToString ()).ToArray (); }, "Pet Type", petTypes));

            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasPicture == true, v => TypedFilter.Details.HasPicture = v == true ? v : (bool?)null, "Has Profile Picture"));
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasReferences == true, v => TypedFilter.Details.HasReferences = v == true ? v : (bool?)null, "Has References"));
        }
    }
}
