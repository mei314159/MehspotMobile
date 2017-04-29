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
    public class SearchModel
    {
        private ViewBadgeProfileTableSource ViewBadgeProfileTableSource;
        private RecommendationsTableSource RecommendationsTableSource;
        public SearchModel (BadgeService badgeService, BadgeSummaryDTO searchBadge, ISearchFilterDTO filter)
        {
            this.BadgeService = badgeService;
            this.SearchBadge = searchBadge;
            this.Filter = filter;
        }
        public BadgeSummaryDTO SearchBadge { get; private set; }
        public BadgeService BadgeService { get; private set; }
        public ISearchFilterDTO Filter { get; private set; }
        public FilterTableSource SearchFilterTableSource { get; private set; }
        public SearchResultTableSource SearchResultTableSource { get; private set; }


        public async Task<ViewBadgeProfileTableSource> GetViewProfileTableSource (string userId)
        {
            await ViewBadgeProfileTableSource.LoadAsync (userId);
            return ViewBadgeProfileTableSource;
        }

        public async Task<RecommendationsTableSource> GetRecommendationsTableSource (string userId, string currentUserId)
        {
            await RecommendationsTableSource.LoadAsync (userId, currentUserId);
            return RecommendationsTableSource;
        }

        public static async Task<SearchModel> GetInstanceAsync (BadgeService badgeService, BadgeSummaryDTO searchBadge)
        {
            FilterTableSource searchFilterTableSource;
            ViewBadgeProfileTableSource viewBadgeProfileTableSource;
            Type resultType;
            switch (searchBadge.BadgeName) {
            case BadgeService.BadgeNames.Babysitter:
                searchFilterTableSource = new BabysitterFilterTableSource (badgeService, searchBadge.BadgeId);
                resultType = typeof (BabysitterSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewBabysitterTableSource (searchBadge.BadgeId, searchBadge.BadgeName, badgeService);
                break;
            case BadgeService.BadgeNames.BabysitterEmployer:
                searchFilterTableSource = new BabysitterEmployerFilterTableSource (badgeService, searchBadge.BadgeId);
                resultType = typeof (BabysitterEmployerSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewBabysitterEmployerTableSource (searchBadge.BadgeId, searchBadge.BadgeName, badgeService);
                break;
            case BadgeService.BadgeNames.Tennis:
                searchFilterTableSource = new TennisFilterTableSource (badgeService, searchBadge.BadgeId);
                resultType = typeof (TennisSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewTennisTableSource (searchBadge.BadgeId, searchBadge.BadgeName, badgeService);
                break;
            case BadgeService.BadgeNames.Golf:
                searchFilterTableSource = new GolfFilterTableSource (badgeService, searchBadge.BadgeId);
                viewBadgeProfileTableSource = new ViewGolfTableSource (searchBadge.BadgeId, searchBadge.BadgeName, badgeService);
                resultType = typeof (GolfSearchResultDTO []);
                break;
            case BadgeService.BadgeNames.Tutor:
                searchFilterTableSource = new TutorFilterTableSource (badgeService, searchBadge.BadgeId);
                resultType = typeof (TutorSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewTutorTableSource (searchBadge.BadgeId, searchBadge.BadgeName, badgeService);
                break;
            case BadgeService.BadgeNames.TutorEmployer:
                searchFilterTableSource = new TutorEmployerFilterTableSource (badgeService, searchBadge.BadgeId);
                resultType = typeof (TutorEmployerSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewTutorEmployerTableSource (searchBadge.BadgeId, searchBadge.BadgeName, badgeService);
                break;
            case BadgeService.BadgeNames.Fitness:
                searchFilterTableSource = new FitnessFilterTableSource (badgeService, searchBadge.BadgeId);
                resultType = typeof (FitnessSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewFitnessTableSource (searchBadge.BadgeId, searchBadge.BadgeName, badgeService);
                break;
            case BadgeService.BadgeNames.PetSitter:
                searchFilterTableSource = new PetSitterFilterTableSource (badgeService, searchBadge.BadgeId);
                resultType = typeof (PetSitterSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewPetSitterTableSource (searchBadge.BadgeId, searchBadge.BadgeName, badgeService);
                break;
            case BadgeService.BadgeNames.PetSitterEmployer:
                searchFilterTableSource = new PetSitterEmployerFilterTableSource (badgeService, searchBadge.BadgeId);
                resultType = typeof (PetSitterEmployerSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewPetSitterEmployerTableSource (searchBadge.BadgeId, searchBadge.BadgeName, badgeService);
                break;
            case BadgeService.BadgeNames.KidsPlayDate:
                searchFilterTableSource = new KidsPlayDateFilterTableSource (badgeService, searchBadge.BadgeId);
                resultType = typeof (KidsPlayDateSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewKidsPlayDateTableSource (searchBadge.BadgeId, searchBadge.BadgeName, badgeService);
                break;
            case BadgeService.BadgeNames.Hobby:
                searchFilterTableSource = new HobbyFilterTableSource (badgeService, searchBadge.BadgeId);
                resultType = typeof (HobbySearchResultDTO []);
                viewBadgeProfileTableSource = new ViewHobbyTableSource (searchBadge.BadgeId, searchBadge.BadgeName, badgeService);
                break;
            case BadgeService.BadgeNames.OtherJobs:
                searchFilterTableSource = new OtherJobsFilterTableSource (badgeService, searchBadge.BadgeId);
                resultType = typeof (OtherJobsSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewOtherJobsTableSource (searchBadge.BadgeId, searchBadge.BadgeName, badgeService);
                break;
            default:
                return null;
            }

            await searchFilterTableSource.InitializeAsync ();

            var model = new SearchModel (badgeService, searchBadge, searchFilterTableSource.Filter);
            model.SearchFilterTableSource = searchFilterTableSource;
            model.SearchResultTableSource = new SearchResultTableSource (badgeService, searchFilterTableSource.Filter, resultType, searchBadge);
            model.ViewBadgeProfileTableSource = viewBadgeProfileTableSource;
            model.RecommendationsTableSource = new RecommendationsTableSource (searchBadge.BadgeId, searchBadge.BadgeName, badgeService);
            return model;
        }
    }
}
