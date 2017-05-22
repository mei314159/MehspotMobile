using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{
    public delegate void GoToMessagingHandler (string userId, string userName);
    public class RecommendationsTableSource : UITableViewSource
    {
        protected readonly int BadgeId;
        protected readonly string BadgeName;
        protected readonly BadgeService badgeService;
        private readonly List<UITableViewCell> cells = new List<UITableViewCell> ();

        string currentUserId;

        public event Action OnWriteReviewButtonTouched;
        public event GoToMessagingHandler OnGoToMessaging;
        public RecommendationsTableSource (BadgeService badgeService, int badgeId, string badgeName)
        {
            BadgeId = badgeId;
            BadgeName = badgeName;
            this.badgeService = badgeService;
        }

        public BadgeRecommendationDTO Recommendations { get; protected set; }

        public async Task LoadAsync (string userId, string currentUserId)
        {
            this.currentUserId = currentUserId;
            cells.Clear ();
            var result = await badgeService.GetBadgeRecommendationsAsync (this.BadgeId, userId);

            if (result.IsSuccess) {
                bool reviewed = false;
                if (result.Data?.Recommendations != null) {
                    this.Recommendations = result.Data;

                    foreach (var item in result.Data.Recommendations) {
                        if (item.FromUserId == currentUserId) {
                            reviewed = true;
                        }
                        cells.Add (RecommendationCell.Create (item));
                    }
                }

                if (!reviewed) {
                    var createRecommendationCell = ButtonCell.Create ("Write Recommendation");
                    createRecommendationCell.OnButtonTouched += CreateRecommendationCell_OnButtonTouched;
                    cells.Insert (0, createRecommendationCell);
                }
            }
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            return cells [indexPath.Row];
        }

        public override nint RowsInSection (UITableView tableview, nint section)
        {
            return cells.Count;
        }

        public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt (indexPath) as RecommendationCell;
            if (cell != null && this.currentUserId != cell.Dto.FromUserId) {
                OnGoToMessaging?.Invoke (cell.Dto.FromUserId, cell.Dto.FromUserName);
            }
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            if (cells.Count > 0) {
                var cell = cells [indexPath.Row];
                return cell.Frame.Height;
            } else {
                return 70;
            }
        }

        void CreateRecommendationCell_OnButtonTouched (Views.Cell.ButtonCell arg1, UIButton arg2)
        {
            OnWriteReviewButtonTouched?.Invoke ();
        }

        internal void AddRecommendation (BadgeUserRecommendationDTO recommendation)
        {
            cells.Add (RecommendationCell.Create (recommendation));
        }

        public void HideCreateButton ()
        {
            cells.RemoveAll (a => a is ButtonCell);
        }
    }
}