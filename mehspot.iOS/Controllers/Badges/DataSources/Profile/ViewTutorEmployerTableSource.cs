using System.Threading.Tasks;
using mehspot.iOS.Views;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{

    public class ViewTutorEmployerTableSource : ViewBadgeProfileTableSource<BadgeProfileDTO<TutorEmployerProfileDTO>>
    {
        public ViewTutorEmployerTableSource (int badgeId, string badgeName, BadgeService badgeService) : base (badgeId, badgeName, badgeService)
        {
        }

        public override Task InitializeAsync (BadgeProfileDTO<TutorEmployerProfileDTO> profile)
        {
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FirstName, "First Name"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.Gender, "Gender"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorEmployerGrade, "Student Grade"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorEmployerSubject, "Subject"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorEmployerSubjectOther, "Other subject or language"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorEmployerCanTravel, "Can travel to locations"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorEmployerPreferredLocation, "Preferred Location"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorEmployerPreferredDate, "Preferred Date"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorEmployerPreferredLength, "Preferred Length"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.AdditionalInfo, "Additional Information"));

            Cells.Add (BooleanEditCell.Create (profile.Details.IsHired, v => { profile.Details.IsHired = v; IsHiredCell_ValueChanged (v); }, "Played Before"));
            Cells.Add (BooleanEditCell.Create (profile.Details.HasReference, v => { profile.Details.HasReference = v; AddReferenceCell_ValueChanged (v); }, "Add Reference"));
            Cells.Add (TextViewCell.Create (profile.Details.ReferenceCount.ToString (), "References Count"));

            return Task.CompletedTask;
        }
    }
}
