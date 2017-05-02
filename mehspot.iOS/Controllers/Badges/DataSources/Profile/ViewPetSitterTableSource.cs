using System.Threading.Tasks;
using mehspot.iOS.Views;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{

    public class ViewPetSitterTableSource : ViewBadgeProfileTableSource<BadgeProfileDTO<PetSitterProfileDTO>>
    {
        public ViewPetSitterTableSource (int badgeId, string badgeName, BadgeService badgeService) : base (badgeId, badgeName, badgeService)
        {
        }

        public override Task InitializeAsync (BadgeProfileDTO<PetSitterProfileDTO> profile)
        {
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FirstName, "First Name"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.AgeRange, "Age Range"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.PetSitterPetType, "Types of pet willing to sit"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.PetSitterPetWeight, "For dogs size up to, pounds"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.PetSitterExpirience + " " + profile.BadgeValues.PetSitterExpirienceOther, "Experience With"));
            Cells.Add (BooleanEditCell.Create (profile.BadgeValues.PetSitterDropOff, (obj) => { }, "Drop off option"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.PetSitterAdditionalQualification, "Additional Qualification"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.PetSitterDogRate, "Dog Rate"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.PetSitterCatRate, "Cat Rate"));
            Cells.Add (BooleanEditCell.Create (profile.BadgeValues.PetSitterMultiplePetDiscount, (obj) => { }, "Multiple pet discount"));
            Cells.Add (BooleanEditCell.Create (profile.BadgeValues.PetSitterMultipleDaysDiscount, (obj) => { }, "Multiple day discount"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.AdditionalInfo, "Additional Information"));

            Cells.Add (BooleanEditCell.Create (profile.Details.IsHired, v => { profile.Details.IsHired = v; IsHiredCell_ValueChanged (v); }, "Played Before"));
            Cells.Add (BooleanEditCell.Create (profile.Details.HasReference, v => { profile.Details.HasReference = v; AddReferenceCell_ValueChanged (v); }, "Add Reference"));
            Cells.Add (TextViewCell.Create (profile.Details.ReferenceCount.ToString (), "References Count"));

            return Task.CompletedTask;
        }
    }
}
