using System;
using System.Threading.Tasks;
using mehspot.iOS.Controllers.Badges.BadgeProfileDataSource;
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
        public string BadgeName { get; set; }
        public BadgeService BadgeService { get; private set; }
        public SearchModel (BadgeService badgeService, string badgeName, ISearchFilterDTO filter)
        {
            this.BadgeService = badgeService;
            this.BadgeName = badgeName;
            this.Filter = filter;
        }

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

        public static async Task<SearchModel> GetInstanceAsync (BadgeService badgeService, string badgeName, int badgeId)
        {
            FilterTableSource searchFilterTableSource;
            ViewBadgeProfileTableSource viewBadgeProfileTableSource;
            Type resultType;
            switch (badgeName) {
            case BadgeService.BadgeNames.Babysitter:
                searchFilterTableSource = new BabysitterFilterTableSource (badgeService, badgeId);
                resultType = typeof (BabysitterSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewBabysitterTableSource (badgeId, badgeName, badgeService);
                break;
            case BadgeService.BadgeNames.BabysitterEmployer:
                searchFilterTableSource = new BabysitterEmployerFilterTableSource (badgeService, badgeId);
                resultType = typeof (BabysitterEmployerSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewBabysitterEmployerTableSource (badgeId, badgeName, badgeService);
                break;
            case BadgeService.BadgeNames.Tennis:
                searchFilterTableSource = new TennisFilterTableSource (badgeService, badgeId);
                resultType = typeof (TennisSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewTennisTableSource (badgeId, badgeName, badgeService);
                break;
            case BadgeService.BadgeNames.Golf:
                searchFilterTableSource = new GolfFilterTableSource (badgeService, badgeId);
                viewBadgeProfileTableSource = new ViewGolfTableSource (badgeId, badgeName, badgeService);
                resultType = typeof (GolfSearchResultDTO []);
                break;
            case BadgeService.BadgeNames.Tutor:
                searchFilterTableSource = new TutorFilterTableSource (badgeService, badgeId);
                resultType = typeof (TutorSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewTutorTableSource (badgeId, badgeName, badgeService);
                break;
            case BadgeService.BadgeNames.TutorEmployer:
                searchFilterTableSource = new TutorEmployerFilterTableSource (badgeService, badgeId);
                resultType = typeof (TutorEmployerSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewTutorEmployerTableSource (badgeId, badgeName, badgeService);
                break;
            case BadgeService.BadgeNames.Fitness:
                searchFilterTableSource = new FitnessFilterTableSource (badgeService, badgeId);
                resultType = typeof (FitnessSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewFitnessTableSource (badgeId, badgeName, badgeService);
                break;
            case BadgeService.BadgeNames.PetSitter:
                searchFilterTableSource = new PetSitterFilterTableSource (badgeService, badgeId);
                resultType = typeof (PetSitterSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewPetSitterTableSource (badgeId, badgeName, badgeService);
                break;
            case BadgeService.BadgeNames.PetSitterEmployer:
                searchFilterTableSource = new PetSitterEmployerFilterTableSource (badgeService, badgeId);
                resultType = typeof (PetSitterEmployerSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewPetSitterEmployerTableSource (badgeId, badgeName, badgeService);
                break;
            case BadgeService.BadgeNames.KidsPlayDate:
                searchFilterTableSource = new KidsPlayDateFilterTableSource (badgeService, badgeId);
                resultType = typeof (KidsPlayDateSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewKidsPlayDateTableSource (badgeId, badgeName, badgeService);
                break;
            case BadgeService.BadgeNames.Hobby:
                searchFilterTableSource = new HobbyFilterTableSource (badgeService, badgeId);
                resultType = typeof (HobbySearchResultDTO []);
                viewBadgeProfileTableSource = new ViewHobbyTableSource (badgeId, badgeName, badgeService);
                break;
            case BadgeService.BadgeNames.OtherJobs:
                searchFilterTableSource = new OtherJobsFilterTableSource (badgeService, badgeId);
                resultType = typeof (OtherJobsSearchResultDTO []);
                viewBadgeProfileTableSource = new ViewOtherJobsTableSource (badgeId, badgeName, badgeService);
                break;
            default:
                return null;
            }

            await searchFilterTableSource.InitializeAsync ();
            var model = new SearchModel (badgeService, badgeName, searchFilterTableSource.Filter);
            model.SearchFilterTableSource = searchFilterTableSource;
            model.SearchResultTableSource = new SearchResultTableSource (badgeService, searchFilterTableSource.Filter, resultType);
            model.ViewBadgeProfileTableSource = viewBadgeProfileTableSource;
            model.RecommendationsTableSource = new RecommendationsTableSource (badgeId, badgeName, badgeService);
            return model;
        }
    }
}
