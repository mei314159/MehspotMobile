using System;
using System.Collections.Generic;
using Foundation;
using mehspot.iOS.Views;
using Mehspot.Core;
using MehSpot.Models.ViewModels;
using MehSpot.Web.ViewModels;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{
    public class BabysitterDataSource: UITableViewSource
    {
        private readonly List<UITableViewCell> cells;
        private readonly BadgeService badgeService;
        private readonly BadgeProfileDTO<BabysitterProfileDTO> profile;

        public BabysitterDataSource (BadgeProfileDTO<BabysitterProfileDTO> profile, BadgeService badgeService)
        {
            this.profile = profile;
            this.badgeService = badgeService;
            cells = new List<UITableViewCell> ();
            cells.Add (BooleanEditCell.Create (profile, a=> a.Values.OwnCar, "Own Car", true));
            cells.Add (BooleanEditCell.Create (profile, a => a.Values.CanDrive, "Can Drive", true));
            cells.Add (TextViewCell.Create (profile.Values.BabysitterCertificationInfo, "Certifications"));
            cells.Add (TextViewCell.Create (profile.Values.BabysitterOtherCertifications, "Other Certifications and  URLs"));
            cells.Add (TextViewCell.Create (profile.Values.BabysitterAdditionalInformation, "Additional Information"));
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
            badgeService.ToggleBadgeEmploymentHistoryAsync (profile.Details.UserId, BadgeService.BadgeNames.Babysitter, !value);
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
