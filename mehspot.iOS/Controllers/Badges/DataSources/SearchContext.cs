using System;
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

		public SearchContext(BadgeService badgeService, BadgeSummaryDTO searchBadge)
		{
			this.badgeService = badgeService;
			this.BadgeSummary = searchBadge;
			Initialize();
		}


		public BadgeSummaryDTO BadgeSummary { get; private set; }
		public ISearchQueryDTO SearchQuery { get; private set; }
		public Type SearchQueryDtoType { get; internal set; }
		public Type SearchResultDtoType { get; private set; }
		public Type ViewProfileDtoType { get; private set; }

		private void Initialize()
		{
			switch (BadgeSummary.BadgeName)
			{
				case BadgeService.BadgeNames.Babysitter:
					SearchQueryDtoType = typeof(SearchBabysitterDTO);
					SearchResultDtoType = typeof(BabysitterSearchResultDTO[]);
					ViewProfileDtoType = typeof(BadgeProfileDTO<BabysitterProfileDTO>);
					break;
				case BadgeService.BadgeNames.BabysitterEmployer:
					SearchQueryDtoType = typeof(SearchBabysitterEmployerDTO);
					SearchResultDtoType = typeof(BabysitterEmployerSearchResultDTO[]);
					ViewProfileDtoType = typeof(BadgeProfileDTO<BabysitterEmployerProfileDTO>);
					break;
				case BadgeService.BadgeNames.Tennis:
					SearchQueryDtoType = typeof(SearchTennisDTO);
					SearchResultDtoType = typeof(TennisSearchResultDTO[]);
					ViewProfileDtoType = typeof(BadgeProfileDTO<TennisProfileDTO>);
					break;
				case BadgeService.BadgeNames.Golf:
					SearchQueryDtoType = typeof(SearchGolfDTO);
					SearchResultDtoType = typeof(GolfSearchResultDTO[]);
					ViewProfileDtoType = typeof(BadgeProfileDTO<GolfProfileDTO>);
					break;
				case BadgeService.BadgeNames.Tutor:
					SearchQueryDtoType = typeof(SearchTutorDTO);
					SearchResultDtoType = typeof(TutorSearchResultDTO[]);
					ViewProfileDtoType = typeof(BadgeProfileDTO<TutorProfileDTO>);
					break;
				case BadgeService.BadgeNames.TutorEmployer:
					SearchQueryDtoType = typeof(SearchTutorEmployerDTO);
					SearchResultDtoType = typeof(TutorEmployerSearchResultDTO[]);
					ViewProfileDtoType = typeof(BadgeProfileDTO<TutorEmployerProfileDTO>);
					break;
				case BadgeService.BadgeNames.Fitness:
					SearchQueryDtoType = typeof(SearchFitnessDTO);
					SearchResultDtoType = typeof(FitnessSearchResultDTO[]);
					ViewProfileDtoType = typeof(BadgeProfileDTO<FitnessProfileDTO>);
					break;
				case BadgeService.BadgeNames.PetSitter:
					SearchQueryDtoType = typeof(SearchPetSitterDTO);
					SearchResultDtoType = typeof(PetSitterSearchResultDTO[]);
					ViewProfileDtoType = typeof(BadgeProfileDTO<PetSitterProfileDTO>);
					break;
				case BadgeService.BadgeNames.PetSitterEmployer:
					SearchQueryDtoType = typeof(SearchPetSitterEmployerDTO);
					SearchResultDtoType = typeof(PetSitterEmployerSearchResultDTO[]);
					ViewProfileDtoType = typeof(BadgeProfileDTO<PetSitterEmployerProfileDTO>);
					break;
				case BadgeService.BadgeNames.KidsPlayDate:
					SearchQueryDtoType = typeof(SearchKidsPlayDateDTO);
					SearchResultDtoType = typeof(KidsPlayDateSearchResultDTO[]);
					ViewProfileDtoType = typeof(BadgeProfileDTO<KidsPlayDateProfileDTO>);
					break;
				case BadgeService.BadgeNames.Friendship:
					SearchQueryDtoType = typeof(SearchFriendshipDTO);
					SearchResultDtoType = typeof(FriendshipSearchResultDTO[]);
					ViewProfileDtoType = typeof(BadgeProfileDTO<FriendshipProfileDTO>);
					break;
				case BadgeService.BadgeNames.OtherJobs:
					SearchQueryDtoType = typeof(SearchOtherJobsDTO);
					SearchResultDtoType = typeof(OtherJobsSearchResultDTO[]);
					ViewProfileDtoType = typeof(BadgeProfileDTO<OtherJobsProfileDTO>);
					break;
				default:
					return;
			}

			SearchQuery = (ISearchQueryDTO)Activator.CreateInstance(SearchQueryDtoType);
			SearchQuery.BadgeId = BadgeSummary.BadgeId;
		}
	}
}