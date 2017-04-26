using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{

    public class RecommendationsTableSource : UITableViewSource
    {
        protected readonly int BadgeId;
        protected readonly string BadgeName;
        protected readonly BadgeService badgeService;

        public RecommendationsTableSource (int badgeId, string badgeName, BadgeService badgeService)
        {
            BadgeId = badgeId;
            BadgeName = badgeName;
            this.badgeService = badgeService;
        }

        public BadgeRecommendationDTO Recommendations { get; protected set; }

        public async Task LoadAsync (string userId)
        {
            var result = await badgeService.GetBadgeRecommendationsAsync (this.BadgeId, userId);
            if (result.IsSuccess) {
                this.Recommendations = result.Data;
            }
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = Recommendations?.Recommendations [indexPath.Row];
            var cell = tableView.DequeueReusableCell (RecommendationCell.Key) as RecommendationCell;
            if (cell != null) {
                cell.Configure (item);
            }

            return cell;
        }

        public override nint RowsInSection (UITableView tableview, nint section)
        {
            return Recommendations?.Recommendations?.Count ?? 0;
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return 70;
        }
    }
}
