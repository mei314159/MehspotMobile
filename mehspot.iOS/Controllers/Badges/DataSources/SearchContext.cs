using System;
using System.Threading.Tasks;
using mehspot.iOS.Controllers.Badges.BadgeProfileDataSource;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using MehSpot.Models.ViewModels;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{
	public class SearchContext
	{
		private BadgeService badgeService;
		private ViewBadgeProfileTableSource ViewBadgeProfileTableSource;

		public SearchContext(BadgeService badgeService, BadgeSummaryDTO searchBadge)
		{
			this.badgeService = badgeService;
			this.BadgeSummary = searchBadge;
			Initialize();
		}


		public BadgeSummaryDTO BadgeSummary { get; private set; }
		public ISearchQueryDTO SearchQuery { get; private set; }
		public Type SearchResultDtoType { get; private set; }

		public async Task<ViewBadgeProfileTableSource> GetViewProfileTableSource(string userId)
		{
			await ViewBadgeProfileTableSource.LoadAsync(userId);
			return ViewBadgeProfileTableSource;
		}

		public void Initialize()
		{
			ViewBadgeProfileTableSource viewBadgeProfileTableSource;

			switch (BadgeSummary.BadgeName)
			{
				case BadgeService.BadgeNames.Babysitter:
					SearchQuery = (SearchBabysitterDTO)Activator.CreateInstance(typeof(SearchBabysitterDTO));
					SearchResultDtoType = typeof(BabysitterSearchResultDTO[]);
					viewBadgeProfileTableSource = new ViewBabysitterTableSource(BadgeSummary.BadgeId, BadgeSummary.BadgeName, badgeService);
					break;
				case BadgeService.BadgeNames.BabysitterEmployer:
					SearchQuery = (SearchBabysitterEmployerDTO)Activator.CreateInstance(typeof(SearchBabysitterEmployerDTO));
					SearchResultDtoType = typeof(BabysitterEmployerSearchResultDTO[]);
					viewBadgeProfileTableSource = new ViewBabysitterEmployerTableSource(BadgeSummary.BadgeId, BadgeSummary.BadgeName, badgeService);
					break;
				case BadgeService.BadgeNames.Tennis:
					SearchQuery = (SearchTennisDTO)Activator.CreateInstance(typeof(SearchTennisDTO));
					SearchResultDtoType = typeof(TennisSearchResultDTO[]);
					viewBadgeProfileTableSource = new ViewTennisTableSource(BadgeSummary.BadgeId, BadgeSummary.BadgeName, badgeService);
					break;
				case BadgeService.BadgeNames.Golf:
					SearchQuery = (SearchGolfDTO)Activator.CreateInstance(typeof(SearchGolfDTO));
					viewBadgeProfileTableSource = new ViewGolfTableSource(BadgeSummary.BadgeId, BadgeSummary.BadgeName, badgeService);
					SearchResultDtoType = typeof(GolfSearchResultDTO[]);
					break;
				case BadgeService.BadgeNames.Tutor:
					SearchQuery = (SearchTutorDTO)Activator.CreateInstance(typeof(SearchTutorDTO));
					SearchResultDtoType = typeof(TutorSearchResultDTO[]);
					viewBadgeProfileTableSource = new ViewTutorTableSource(BadgeSummary.BadgeId, BadgeSummary.BadgeName, badgeService);
					break;
				case BadgeService.BadgeNames.TutorEmployer:
					SearchQuery = (SearchTutorEmployerDTO)Activator.CreateInstance(typeof(SearchTutorEmployerDTO));
					SearchResultDtoType = typeof(TutorEmployerSearchResultDTO[]);
					viewBadgeProfileTableSource = new ViewTutorEmployerTableSource(BadgeSummary.BadgeId, BadgeSummary.BadgeName, badgeService);
					break;
				case BadgeService.BadgeNames.Fitness:
					SearchQuery = (SearchFitnessDTO)Activator.CreateInstance(typeof(SearchFitnessDTO));
					SearchResultDtoType = typeof(FitnessSearchResultDTO[]);
					viewBadgeProfileTableSource = new ViewFitnessTableSource(BadgeSummary.BadgeId, BadgeSummary.BadgeName, badgeService);
					break;
				case BadgeService.BadgeNames.PetSitter:
					SearchQuery = (SearchPetSitterDTO)Activator.CreateInstance(typeof(SearchPetSitterDTO));
					SearchResultDtoType = typeof(PetSitterSearchResultDTO[]);
					viewBadgeProfileTableSource = new ViewPetSitterTableSource(BadgeSummary.BadgeId, BadgeSummary.BadgeName, badgeService);
					break;
				case BadgeService.BadgeNames.PetSitterEmployer:
					SearchQuery = (SearchPetSitterEmployerDTO)Activator.CreateInstance(typeof(SearchPetSitterEmployerDTO));
					SearchResultDtoType = typeof(PetSitterEmployerSearchResultDTO[]);
					viewBadgeProfileTableSource = new ViewPetSitterEmployerTableSource(BadgeSummary.BadgeId, BadgeSummary.BadgeName, badgeService);
					break;
				case BadgeService.BadgeNames.KidsPlayDate:
					SearchQuery = (SearchKidsPlayDateDTO)Activator.CreateInstance(typeof(SearchKidsPlayDateDTO));
					SearchResultDtoType = typeof(KidsPlayDateSearchResultDTO[]);
					viewBadgeProfileTableSource = new ViewKidsPlayDateTableSource(BadgeSummary.BadgeId, BadgeSummary.BadgeName, badgeService);
					break;
				case BadgeService.BadgeNames.Friendship:
					SearchQuery = (SearchFriendshipDTO)Activator.CreateInstance(typeof(SearchFriendshipDTO));
					SearchResultDtoType = typeof(FriendshipSearchResultDTO[]);
					viewBadgeProfileTableSource = new ViewFriendshipTableSource(BadgeSummary.BadgeId, BadgeSummary.BadgeName, badgeService);
					break;
				case BadgeService.BadgeNames.OtherJobs:
					SearchQuery = (SearchOtherJobsDTO)Activator.CreateInstance(typeof(SearchOtherJobsDTO));
					SearchResultDtoType = typeof(OtherJobsSearchResultDTO[]);
					viewBadgeProfileTableSource = new ViewOtherJobsTableSource(BadgeSummary.BadgeId, BadgeSummary.BadgeName, badgeService);
					break;
				default:
					return;
			}

			this.ViewBadgeProfileTableSource = viewBadgeProfileTableSource;
		}
	}
}