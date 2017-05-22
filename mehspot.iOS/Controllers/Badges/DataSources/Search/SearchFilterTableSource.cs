using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using mehspot.iOS.Views;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Filter.Search;
using Mehspot.Core.Services;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{
	public class SearchFilterTableSource : UITableViewSource
	{
		public List<UITableViewCell> Cells = new List<UITableViewCell>();
		private CellsSource cellsSource;

		public SearchFilterTableSource(BadgeService badgeService, int badgeId)
		{
			cellsSource = new CellsSource(badgeService, badgeId);
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var item = Cells[indexPath.Row];
			return item;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return Cells.Count;
		}

		public override string TitleForHeader(UITableView tableView, nint section)
		{
			if (section == 0)
			{
				return "Filter";
			}

			return string.Empty;
		}

		public async Task Initialize(object filter) {
			this.Cells.Clear();
			this.Cells.AddRange(await cellsSource.CreateCells(filter));
		}
	}
}