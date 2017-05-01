using System.Threading.Tasks;
using mehspot.iOS.Views;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{

    public class ViewFriendshipTableSource : ViewBadgeProfileTableSource<BadgeProfileDTO<FriendshipProfileDTO>>
    {
        public ViewFriendshipTableSource (int badgeId, string badgeName, BadgeService badgeService) : base (badgeId, badgeName, badgeService)
        {
        }

        public override Task InitializeAsync (BadgeProfileDTO<FriendshipProfileDTO> profile)
        {
            if (profile.BadgeValues.HobbyType != null && !string.IsNullOrEmpty (profile.BadgeValues.HobbyOther)) {
                profile.BadgeValues.HobbyType = profile.BadgeValues.HobbyType.Replace ("Other", profile.BadgeValues.HobbyOther);
            }

            Cells.Add (TextViewCell.Create (profile.BadgeValues.FirstName, "First Name"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.HobbyType, "Hobby"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.Gender, "Gender"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.HobbyAgeRange, "Age Range"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.HobbyPreferredDateTime, "Preferred Date and Time"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.HobbyPreferredLocation, "Preferred Location"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FriendshipFamilyComposition, "Family Composition"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FriendshipProfession, "Profession"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FriendshipLookingFor, "Looking For"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.HobbyAdditionalInformation, "Additional Information"));

            Cells.Add (BooleanEditCell.Create (profile.Details.IsHired, v => { profile.Details.IsHired = v; IsHiredCell_ValueChanged (v); }, "Played Before"));
            Cells.Add (BooleanEditCell.Create (profile.Details.HasReference, v => { profile.Details.HasReference = v; AddReferenceCell_ValueChanged (v); }, "Add Reference"));
            Cells.Add (TextViewCell.Create (profile.Details.ReferenceCount.ToString (), "References Count"));

            return Task.CompletedTask;
        }
    }
}
