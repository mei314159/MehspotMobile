using System.Threading.Tasks;
using mehspot.iOS.Views;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{

    public class ViewPetSitterEmployerTableSource : ViewBadgeProfileTableSource<BadgeProfileDTO<PetSitterEmployerProfileDTO>>
    {
        public ViewPetSitterEmployerTableSource (int badgeId, string badgeName, BadgeService badgeService) : base (badgeId, badgeName, badgeService)
        {
        }

        public override Task InitializeAsync (BadgeProfileDTO<PetSitterEmployerProfileDTO> profile)
        {
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FirstName, "First Name"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.PetSitterEmployerPetType, "Types of pet"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.PetSitterEmployerPetWeight, "For dogs size up to, pounds"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.PetSitterEmployerAdditionalPetInformation, "Experience With"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.PetSitterEmployerExpirienceRequiredInformation, "Sitter experience with animal required"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.AdditionalInfo, "Additional Information"));

            Cells.Add (BooleanEditCell.Create (profile.Details.IsHired, v => { profile.Details.IsHired = v; IsHiredCell_ValueChanged (v); }, "Played Before"));
            Cells.Add (BooleanEditCell.Create (profile.Details.HasReference, v => { profile.Details.HasReference = v; AddReferenceCell_ValueChanged (v); }, "Add Reference"));
            Cells.Add (TextViewCell.Create (profile.Details.ReferenceCount.ToString (), "References Count"));

            return Task.CompletedTask;
        }
    }
}
