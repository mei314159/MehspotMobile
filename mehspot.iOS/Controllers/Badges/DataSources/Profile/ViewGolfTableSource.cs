using System.Threading.Tasks;
using mehspot.iOS.Views;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{

    public class ViewGolfTableSource : ViewBadgeProfileTableSource<BadgeProfileDTO<GolfProfileDTO>>
    {
        public ViewGolfTableSource (int badgeId, string badgeName, BadgeService badgeService) : base (badgeId, badgeName, badgeService)
        {
        }

        public override Task InitializeAsync (BadgeProfileDTO<GolfProfileDTO> profile)
        {
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FirstName, "Player First Name"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.Gender, "Gender"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.GolfAgeGroup, "Age Group"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.GolfHandicap, "Handicap"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.GolfHomeCourseZip, "Home course zip"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.GolfHomeCourseSubdivision, "Home course subdivision"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.GolfHomeCourseType, "Home Course Type"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.GolfHomeCourseGreenFeeRange, "Green Fee Range"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.GolfGreenFeeRangeWTP, "Green Fee Amount"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.GolfHomeCourtPreference, "Preference to play at home court"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.GolfAdditionalInformation, "Additional Information"));
            Cells.Add (BooleanEditCell.Create (profile.Details.IsHired, v => { profile.Details.IsHired = v; IsHiredCell_ValueChanged (v); }, "Played Before"));
            Cells.Add (BooleanEditCell.Create (profile.Details.HasReference, v => { profile.Details.HasReference = v; AddReferenceCell_ValueChanged (v); }, "Add Reference"));
            Cells.Add (TextViewCell.Create (profile.Details.ReferenceCount.ToString (), "References Count"));

            return Task.CompletedTask;
        }
    }

    



}
