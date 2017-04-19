using System.Threading.Tasks;
using mehspot.iOS.Views;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{

    public class ViewKidsPlayDateTableSource : ViewBadgeProfileTableSource<BadgeProfileDTO<KidsPlayDateProfileDTO>>
    {
        public ViewKidsPlayDateTableSource (int badgeId, string badgeName, BadgeService badgeService) : base (badgeId, badgeName, badgeService)
        {
        }

        public override Task InitializeAsync (BadgeProfileDTO<KidsPlayDateProfileDTO> profile)
        {
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FirstName, "Kid First Name"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.KidsPlayDateKidAge, "Kid Age"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.Gender, "Gender"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.KidsPlayDateRace, "Race"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.KidsPlayDateParentsInformation, "Parents Information"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.KidsPlayDateDesiredDateAndTime, "Desired Date and Time"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.KidsPlayDateDesiredLocation, "Desired Location"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.KidsPlayDateDesiredGender, "Desired Gender"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.KidsPlayDateDesiredAgeRange, "Desired Age Rande"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.KidsPlayDateSpokenLanguage, "Desired Spoken Language"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.KidsPlayDatePreferredMethodOfPlay, "Preferred Method of Play"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.KidsPlayDateMyChild, "My child"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.KidsPlayDateAdditionalInformation, "Additional Information"));

            Cells.Add (BooleanEditCell.Create (profile.Details.IsHired, v => { profile.Details.IsHired = v; IsHiredCell_ValueChanged (v); }, "Played Before"));
            Cells.Add (BooleanEditCell.Create (profile.Details.HasReference, v => { profile.Details.HasReference = v; AddReferenceCell_ValueChanged (v); }, "Add Reference"));
            Cells.Add (TextViewCell.Create (profile.Details.ReferenceCount.ToString (), "References Count"));

            return Task.CompletedTask;
        }
    }

    
}
