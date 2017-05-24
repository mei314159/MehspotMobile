using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Foundation;
using Mehspot.iOS.Controllers.Badges.DataSources.Search;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using UIKit;

namespace Mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{
	public class ViewBadgeProfileTableSource : UITableViewSource
	{
		private readonly CellsSource cellsSource;

		protected readonly List<UITableViewCell> Cells = new List<UITableViewCell>();

		private readonly Type resultType;
		protected readonly int BadgeId;
		protected readonly string BadgeName;
		protected readonly BadgeService badgeService;

		public ViewBadgeProfileTableSource(int badgeId, string badgeName, BadgeService badgeService)
		{
			BadgeId = badgeId;
			BadgeName = badgeName;
			this.badgeService = badgeService;
			cellsSource = new CellsSource(badgeService, badgeId);
			cellsSource.CellChanged += CellsSource_CellChanged;

			var genericParameter = Assembly.GetAssembly(typeof(IBadgeProfileValues))
									 .GetTypes()
									 .FirstOrDefault(a => a
                                     .GetCustomAttribute<ViewProfileDtoAttribute>()?.BadgeName == BadgeName);
			resultType = typeof(BadgeProfileDTO<>).MakeGenericType(genericParameter);
		}

		public IBadgeProfileDTO Profile { get; protected set; }


		public async Task<bool> LoadAsync(string userId)
		{
			var result = await badgeService.GetBadgeProfileAsync(this.BadgeId, userId, resultType);
			if (result.IsSuccess)
			{
				this.Profile = result.Data;
				Cells.Clear();
				await InitializeAsync(result.Data);
			}

			return result.IsSuccess;
		}

		private async Task InitializeAsync(IBadgeProfileDTO searchQuery)
		{
			this.Cells.Clear();
			this.Cells.AddRange(await cellsSource.CreateCells(searchQuery));
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

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return Cells[indexPath.Row].Frame.Height;
		}

		void CellsSource_CellChanged(object obj, string propertyName, object value)
		{
			if (obj is BadgeProfileDetailsDTO)
			{
				if (propertyName == nameof(BadgeProfileDetailsDTO.IsHired))
				{
					badgeService.ToggleBadgeEmploymentHistoryAsync(Profile.Details.UserId, this.BadgeId, !(bool)value);

				}
				else if (propertyName == nameof(BadgeProfileDetailsDTO.HasReference))
				{
					var dto = new BadgeUserDescriptionDTO
					{
						EmployeeId = Profile.Details.UserId,
						BadgeName = this.BadgeName,
						Delete = !(bool)value,
						Type = BadgeDescriptionTypeEnum.Reference
					};

					ToggleBadgeUserDescriptionAsync(dto);
				}
			}
		}

		public Task<Result> ToggleBadgeUserDescriptionAsync(BadgeUserDescriptionDTO dto)
		{
			return badgeService.ToggleBadgeUserDescriptionAsync(dto);
		}
	}
}
