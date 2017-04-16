using System.Threading.Tasks;
using mehspot.iOS.Views;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{

    public class ViewOtherJobsTableSource : ViewBadgeProfileTableSource<BadgeProfileDTO<OtherJobsProfileDTO>>
    {
        public ViewOtherJobsTableSource (int badgeId, string badgeName, BadgeService badgeService) : base (badgeId, badgeName, badgeService)
        {
        }

        public override Task InitializeAsync (BadgeProfileDTO<OtherJobsProfileDTO> profile)
        {
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FirstName, "Kid First Name"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.OtherJobsType, "Job"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.Gender, "Gender"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.OtherJobsAgeRange, "Age Range"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.OtherJobsPreferredDateTime, "Preferred Date and Time"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.OtherJobsLicense, "License"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.OtherJobsExperiencey, "Experience"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.OtherJobsReference, "Reference"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.OtherJobsAdditionalContactInfo, "Contact Info"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.OtherJobsAdditionalInformation, "Additional Information"));

            Cells.Add (BooleanEditCell.Create (profile.Details.IsHired, v => { profile.Details.IsHired = v; IsHiredCell_ValueChanged (v); }, "Played Before"));
            Cells.Add (BooleanEditCell.Create (profile.Details.HasReference, v => { profile.Details.HasReference = v; AddReferenceCell_ValueChanged (v); }, "Add Reference"));
            Cells.Add (TextViewCell.Create (profile.Details.ReferenceCount.ToString (), "References Count"));

            return Task.CompletedTask;
        }
    }
}
