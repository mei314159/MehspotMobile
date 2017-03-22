using System;
using System.Collections.Generic;
using Foundation;
using mehspot.iOS.Views;
using MehSpot.Models.ViewModels;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{
    public class BabysitterDataSource: UITableViewSource
    {
        private List<UITableViewCell> cells;

        public BabysitterDataSource (BadgeProfileDTO<BabysitterProfileDTO> profile)
        {
            cells = new List<UITableViewCell> ();
            cells.Add (TextViewCell.Create (profile.Details.SubdivisionName.Trim(), "Subdivision"));
            cells.Add (TextViewCell.Create ($"${profile.Values.HourlyRate}/hr", "Hourly Rate"));
            cells.Add (TextViewCell.Create (profile.Values.AgeRange, "Age Range"));
            cells.Add (BooleanEditCell.Create (profile, a=> a.Values.OwnCar, "Own Car", true));
            cells.Add (BooleanEditCell.Create (profile, a => a.Values.CanDrive, "Can Drive", true));
            cells.Add (TextViewCell.Create (profile.Values.BabysitterCertificationInfo, "Certifications"));
            cells.Add (TextViewCell.Create (profile.Values.BabysitterOtherCertifications, "Other Certifications and  URLs"));
            cells.Add (TextViewCell.Create (profile.Values.BabysitterAdditionalInformation, "Additional Information"));
            cells.Add (BooleanEditCell.Create (profile, a => a.Details.IsHired, "Hired Before", true)); // TODO POST request
            cells.Add (BooleanEditCell.Create (profile, a => a.Details.HasReference, "Has Reference", true));
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = cells [indexPath.Row];
            return item;
        }

        public override nint RowsInSection (UITableView tableView, nint section)
        {
            return cells.Count;
        }


        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return cells [indexPath.Row].Frame.Height;
        }
    }
}
