using System.Threading.Tasks;
using mehspot.iOS.Views;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{

    public class ViewTennisTableSource : ViewBadgeProfileTableSource<TennisProfileDTO>
    {
        public ViewTennisTableSource (int badgeId, string badgeName, BadgeService badgeService) : base (badgeId, badgeName, badgeService)
        {
        }

        public override Task InitializeAsync (BadgeProfileDTO<TennisProfileDTO> profile)
        {
            Cells.Add (TextViewCell.Create (profile.BadgeValues.Gender, "Gender"));
            Cells.Add (BooleanEditCell.Create (profile.BadgeValues.HasCourt, v => { }, "HasCourt", true));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.SkillLevel, "Skill Level"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.PreferredSide, "Preferred Side"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TennisZip, "Tennis Zip"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TennisSubdivision, "Tennis Subdivision"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TennisAgeRange, "Tennis Age Range"));

            Cells.Add (BooleanEditCell.Create (profile.Details.IsHired, v => { profile.Details.IsHired = v; IsHiredCell_ValueChanged (v); }, "Hired Before"));
            Cells.Add (BooleanEditCell.Create (profile.Details.HasReference, v => { profile.Details.HasReference = v; AddReferenceCell_ValueChanged (v); }, "Add Reference"));
            Cells.Add (TextViewCell.Create (profile.Details.ReferenceCount.ToString (), "References Count"));

            return Task.CompletedTask;
        }
    }

}
