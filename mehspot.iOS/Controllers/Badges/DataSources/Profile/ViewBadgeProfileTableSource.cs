using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{
    public abstract class ViewBadgeProfileTableSource : UITableViewSource
    {
        protected readonly List<UITableViewCell> Cells = new List<UITableViewCell> ();

        private readonly int BadgeId;
        private readonly string BadgeName;
        private readonly BadgeService badgeService;

        public readonly IBadgeProfileDTO Profile;

        public ViewBadgeProfileTableSource (IBadgeProfileDTO profile, int badgeId, string badgeName, BadgeService badgeService)
        {
            Profile = profile;
            BadgeId = badgeId;
            BadgeName = badgeName;
            this.badgeService = badgeService;
        }

        public abstract Task InitializeAsync ();

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = Cells [indexPath.Row];
            return item;
        }

        public override nint RowsInSection (UITableView tableview, nint section)
        {
            return Cells.Count;
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return Cells [indexPath.Row].Frame.Height;
        }

        public virtual void IsHiredCell_ValueChanged (bool value)
        {
            badgeService.ToggleBadgeEmploymentHistoryAsync (Profile.Details.UserId, this.BadgeId, !value);
        }

        public virtual void AddReferenceCell_ValueChanged (bool value)
        {
            var dto = new BadgeUserDescriptionDTO {
                EmployeeId = Profile.Details.UserId,
                BadgeName = this.BadgeName,
                Delete = !value,
                Type = BadgeDescriptionTypeEnum.Reference
            };

            ToggleBadgeUserDescriptionAsync (dto);
        }

        public Task<Result> ToggleBadgeUserDescriptionAsync (BadgeUserDescriptionDTO dto)
        {
            return badgeService.ToggleBadgeUserDescriptionAsync (dto);
        }
    }

}
