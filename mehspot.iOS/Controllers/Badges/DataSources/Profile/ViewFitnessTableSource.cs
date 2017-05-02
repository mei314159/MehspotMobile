using System.Threading.Tasks;
using mehspot.iOS.Views;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{

    public class ViewFitnessTableSource : ViewBadgeProfileTableSource<BadgeProfileDTO<FitnessProfileDTO>>
    {
        public ViewFitnessTableSource (int badgeId, string badgeName, BadgeService badgeService) : base (badgeId, badgeName, badgeService)
        {
        }

        public override Task InitializeAsync (BadgeProfileDTO<FitnessProfileDTO> profile)
        {
            if (profile.BadgeValues.FitnessType != null && !string.IsNullOrEmpty (profile.BadgeValues.FitnessOther)) {
                profile.BadgeValues.FitnessType = profile.BadgeValues.FitnessType.Replace ("Other", profile.BadgeValues.FitnessOther);
            }

            Cells.Add (TextViewCell.Create (profile.BadgeValues.FirstName, "First Name"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.Gender, "Gender"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FitnessType, "Fitness Type"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FitnessAgeRange, "Age Range"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FitnessPreferredLocation, "Preferred Location"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FitnessPreferredDateTime, "Preferred Date"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FitnessPreferredFrequency, "Preferred Frequency"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FitnessPreferredGender, "Preferred Gender"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FitnessAdditionalInformation, "Additional Information"));

            Cells.Add (BooleanEditCell.Create (profile.Details.IsHired, v => { profile.Details.IsHired = v; IsHiredCell_ValueChanged (v); }, "Played Before"));
            Cells.Add (BooleanEditCell.Create (profile.Details.HasReference, v => { profile.Details.HasReference = v; AddReferenceCell_ValueChanged (v); }, "Add Reference"));
            Cells.Add (TextViewCell.Create (profile.Details.ReferenceCount.ToString (), "References Count"));

            return Task.CompletedTask;
        }
    }
}
