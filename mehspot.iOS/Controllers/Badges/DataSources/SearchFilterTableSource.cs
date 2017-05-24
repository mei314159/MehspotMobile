using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using UIKit;

namespace Mehspot.iOS.Controllers.Badges.DataSources.Search
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

		public async Task Initialize(ISearchQueryDTO searchQuery)
		{
			this.Cells.Clear();
			this.Cells.AddRange(await cellsSource.CreateCells(searchQuery));
		}
	}
}