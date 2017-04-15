using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mehspot.iOS.Views;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{

    public class TutorEmployerFilterTableSource : SearchFilterTableSource<SearchTutorEmployerDTO>
    {
        public TutorEmployerFilterTableSource (BadgeService badgeService, int badgeId) : base (badgeService, badgeId)
        {
        }

        public override async Task InitializeAsync ()
        {
            var grades = await GetOptionsAsync (BadgeService.BadgeKeys.TutorGrade);
            var subjects = await GetOptionsAsync (BadgeService.BadgeKeys.TutorSubject);
            this.Cells.Add (SliderCell.Create (TypedFilter, a => a.Details.DistanceFrom, "Max Distance", 0, 200));
            var zipCell = TextEditCell.Create (TypedFilter.Details.ZipCode, a => TypedFilter.Details.ZipCode = a, "Zip");
            zipCell.Mask = "#####";
            this.Cells.Add (zipCell);

            this.Cells.Add (SliderCell.Create (TypedFilter, a => a.HourlyRate, "Max Hourly Rate ($)", 0, 200));
            this.Cells.Add (PickerCell.CreateMultiselect<int?> (new int? [] { }, (property) => { TypedFilter.Subjects = property?.Select (a => a.ToString ()).ToArray (); }, "Subjects", subjects));
            this.Cells.Add (PickerCell.Create (TypedFilter.Grade, (property) => { TypedFilter.Grade = property; }, "Min Grade", grades));
            this.Cells.Add (PickerCell.Create (TypedFilter.MaxGrade, (property) => { TypedFilter.MaxGrade = property; }, "Max Grade", grades));

            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasPicture == true, v => TypedFilter.Details.HasPicture = v == true ? v : (bool?)null, "Has Profile Picture"));
            this.Cells.Add (BooleanEditCell.Create (TypedFilter.Details.HasReferences == true, v => TypedFilter.Details.HasReferences = v == true ? v : (bool?)null, "Has References"));
        }
    }

}
