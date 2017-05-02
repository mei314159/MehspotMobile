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

        protected readonly int BadgeId;
        protected readonly string BadgeName;
        protected readonly BadgeService badgeService;


        public ViewBadgeProfileTableSource (int badgeId, string badgeName, BadgeService badgeService)
        {
            BadgeId = badgeId;
            BadgeName = badgeName;
            this.badgeService = badgeService;
        }

        public IBadgeProfileDTO Profile { get; protected set; }

        public abstract Task LoadAsync (string userId);

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

        public virtual void IsEnabledCell_ValueChanged (bool value)
        {
            badgeService.ToggleEnabledState (this.BadgeId, value);
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

    public abstract class ViewBadgeProfileTableSource<TProfile> : ViewBadgeProfileTableSource where TProfile : IBadgeProfileDTO
    {

        public ViewBadgeProfileTableSource (int badgeId, string badgeName, BadgeService badgeService) : base (badgeId, badgeName, badgeService)
        {
        }

        public override async Task LoadAsync (string userId)
        {
            var result = await badgeService.GetBadgeProfileAsync<TProfile> (this.BadgeId, userId);
            if (result.IsSuccess) {
                this.Profile = result.Data;
                Cells.Clear ();
                await InitializeAsync (result.Data);
            }
        }

        public abstract Task InitializeAsync (TProfile profile);
    }
}
