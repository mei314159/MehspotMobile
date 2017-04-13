using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using Mehspot.Core.DTO.Search;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{
    public abstract class SearchTableDataSource: UITableViewSource
    {
        protected List<UITableViewCell> Cells { get; set; } = new List<UITableViewCell> ();

        public ISearchFilterDTO Filter { get; }

        public SearchTableDataSource (ISearchFilterDTO filter)
        {
            this.Filter = filter;
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

        public override string TitleForHeader (UITableView tableView, nint section)
        {
            if (section == 0) {
                return "Filter";
            }

            return string.Empty;
        }
    }


}
