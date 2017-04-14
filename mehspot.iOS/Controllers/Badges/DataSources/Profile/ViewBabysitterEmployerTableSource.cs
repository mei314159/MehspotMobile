using System.Threading.Tasks;
using mehspot.iOS.Views;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{

    public class ViewBabysitterEmployerTableSource : ViewBadgeProfileTableSource<BabysitterEmployerProfileDTO>
    {
        public ViewBabysitterEmployerTableSource (int badgeId, string badgeName, BadgeService badgeService) : base (badgeId, badgeName, badgeService)
        {
        }

        public override Task InitializeAsync (BadgeProfileDTO<BabysitterEmployerProfileDTO> profile)
        {
            Cells.Add (BooleanEditCell.Create (profile.BadgeValues.OwnCar, v => { }, "Own Car", true));
            Cells.Add (BooleanEditCell.Create (profile.BadgeValues.CanDrive, v => { }, "Can Drive", true));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.Description, "Additional Information"));
            Cells.Add (BooleanEditCell.Create (profile.Details.IsHired, v => { profile.Details.IsHired = v; IsHiredCell_ValueChanged (v); }, "Hired Before"));
            Cells.Add (BooleanEditCell.Create (profile.Details.HasReference, v => { profile.Details.HasReference = v; AddReferenceCell_ValueChanged (v); }, "Add Reference"));
            Cells.Add (TextViewCell.Create (profile.Details.ReferenceCount.ToString (), "References Count"));

            return Task.CompletedTask;
        }
    }
}
