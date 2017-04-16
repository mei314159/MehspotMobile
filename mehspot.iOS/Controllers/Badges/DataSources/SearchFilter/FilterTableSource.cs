using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{
    public abstract class FilterTableSource : UITableViewSource
    {
        protected readonly List<UITableViewCell> Cells = new List<UITableViewCell> ();

        public abstract Task InitializeAsync ();

        public abstract ISearchFilterDTO Filter { get; }

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

    public abstract class SearchFilterTableSource<TFilter> : FilterTableSource where TFilter : ISearchFilterDTO, new()
    {
        readonly BadgeService badgeService;

        public TFilter TypedFilter { get; private set; }

        public override ISearchFilterDTO Filter {
            get {
                return TypedFilter;
            }
        }

        public SearchFilterTableSource (BadgeService badgeService, int badgeId)
        {
            this.badgeService = badgeService;
            this.TypedFilter = new TFilter ();
            TypedFilter.BadgeId = badgeId;
        }

        protected virtual Task<KeyValuePair<int?, string> []> GetAgeRangesAsync ()
        {
            return GetOptionsAsync (BadgeService.BadgeKeys.AgeRange);
        }

        protected async Task<KeyValuePair<int?, string> []> GetOptionsAsync (string key, bool skipFirst = false)
        {
            var result = await badgeService.GetBadgeKeysAsync (this.TypedFilter.BadgeId, key);
            if (result.IsSuccess) {
                var data = skipFirst ? result.Data.Skip (1) : result.Data;
                return data.Select (a => new KeyValuePair<int?, string> (a.Id, a.Name)).ToArray ();
            }

            return null;
        }
    }
}
