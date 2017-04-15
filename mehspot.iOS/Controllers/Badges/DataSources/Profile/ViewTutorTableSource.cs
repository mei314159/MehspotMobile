using System.Threading.Tasks;
using mehspot.iOS.Views;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{

    public class ViewTutorTableSource : ViewBadgeProfileTableSource<TutorProfileDTO>
    {
        public ViewTutorTableSource (int badgeId, string badgeName, BadgeService badgeService) : base (badgeId, badgeName, badgeService)
        {
        }

        public override Task InitializeAsync (BadgeProfileDTO<TutorProfileDTO> profile)
        {
            Cells.Add (TextViewCell.Create (profile.BadgeValues.FirstName, "Tutor First Name"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.Gender, "Gender"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorGrade, "Tutor Grade"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorSubject, "Subject"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorSubjectOther, "Other subject or language"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.HourlyRate, "Hourly Rate"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorCanTravel, "Can travel to locations"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorPreferredLocation, "Preferred Location"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorPreferredDate, "Preferred Date"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.TutorPreferredLength, "Preferred Length"));
            Cells.Add (TextViewCell.Create (profile.BadgeValues.AdditionalInfo, "Additional Information"));

            Cells.Add (BooleanEditCell.Create (profile.Details.IsHired, v => { profile.Details.IsHired = v; IsHiredCell_ValueChanged (v); }, "Played Before"));
            Cells.Add (BooleanEditCell.Create (profile.Details.HasReference, v => { profile.Details.HasReference = v; AddReferenceCell_ValueChanged (v); }, "Add Reference"));
            Cells.Add (TextViewCell.Create (profile.Details.ReferenceCount.ToString (), "References Count"));

            return Task.CompletedTask;
        }
    }
}
