using System;
using System.Collections.Generic;
using Foundation;
using mehspot.iOS.Views;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{
    public class ViewBabysitterDataSource: UITableViewSource
    {
        private readonly List<UITableViewCell> cells;

        private int BadgeId;
        private readonly BadgeService badgeService;
        private readonly BadgeProfileDTO<BabysitterProfileDTO> profile;

        public ViewBabysitterDataSource (BadgeProfileDTO<BabysitterProfileDTO> profile, int badgeId, BadgeService badgeService)
        {
            this.BadgeId = badgeId;
            this.profile = profile;
            this.badgeService = badgeService;
            cells = new List<UITableViewCell> ();
            cells.Add (BooleanEditCell.Create (profile, a=> a.BadgeValues.OwnCar, "Own Car", true));
            cells.Add (BooleanEditCell.Create (profile, a => a.BadgeValues.CanDrive, "Can Drive", true));
            cells.Add (TextViewCell.Create (profile.BadgeValues.BabysitterCertificationInfo, "Certifications"));
            cells.Add (TextViewCell.Create (profile.BadgeValues.BabysitterOtherCertifications, "Other Certifications and  URLs"));
            cells.Add (TextViewCell.Create (profile.BadgeValues.BabysitterAdditionalInformation, "Additional Information"));
            var isHiredCell = BooleanEditCell.Create (profile, a => a.Details.IsHired, "Hired Before");
            cells.Add (isHiredCell); // TODO POST request
            var addReferenceCell = BooleanEditCell.Create (profile, a => a.Details.HasReference, "Add Reference");
            cells.Add (addReferenceCell);
            cells.Add (TextViewCell.Create (profile.Details.ReferenceCount.ToString (), "References Count"));
            isHiredCell.ValueChanged += IsHiredCell_ValueChanged;
            addReferenceCell.ValueChanged += AddReferenceCell_ValueChanged; 
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = cells [indexPath.Row];
            return item;
        }

        public override nint RowsInSection (UITableView tableview, nint section)
        {
            return cells.Count;
        }


        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return cells [indexPath.Row].Frame.Height;
        }

        void IsHiredCell_ValueChanged (bool value)
        {
            badgeService.ToggleBadgeEmploymentHistoryAsync (profile.Details.UserId, this.BadgeId, !value);
        }

        void AddReferenceCell_ValueChanged (bool value)
        {
            var dto = new BadgeUserDescriptionDTO {
                EmployeeId = profile.Details.UserId,
                BadgeName = BadgeService.BadgeNames.Babysitter,
                Delete = !value,
                Type = BadgeDescriptionTypeEnum.Reference
            };

            badgeService.ToggleBadgeUserDescriptionAsync (dto);
        }
    }
}
