using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mehspot.iOS.Views;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{

    public class BabysitterFilterTableSource : SearchFilterTableSource<SearchBabysitterDTO>
    {
        public BabysitterFilterTableSource (BadgeService badgeService, int badgeId) : base (badgeService, badgeId)
        {
        }

        public override async Task InitializeAsync ()
        {
            var ageRanges = await GetOptionsAsync (BadgeService.BadgeKeys.AgeRange);
            this.Cells.Add (SliderCell.Create (TypedFilter, a => a.Details.DistanceFrom, "Max Distance", 0, 200));
            this.Cells.Add (SliderCell.Create (TypedFilter, a => a.HourlyRate, "Max Hourly Rate ($)", 0, 200));
            var zipCell = TextEditCell.Create (TypedFilter.Details.ZipCode, a => TypedFilter.Details.ZipCode = a, "Zip");
            zipCell.Mask = "#####";
            this.Cells.Add (zipCell);
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.OwnCar == true, v => TypedFilter.OwnCar = v == true ? v : (bool?)null, "Has Car"));
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasPicture == true, v => TypedFilter.Details.HasPicture = v == true ? v : (bool?)null, "Has Profile Picture"));
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasReferences == true, v => TypedFilter.Details.HasReferences = v == true ? v : (bool?)null, "Has References"));
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.HasCertification == true, v => TypedFilter.HasCertification = v == true ? v : (bool?)null, "Has Certification"));
            this.Cells.Add (PickerCell.Create (TypedFilter.AgeRange, (property) => { TypedFilter.AgeRange = property; }, "Age Range", ageRanges));
        }
    }

    
}
