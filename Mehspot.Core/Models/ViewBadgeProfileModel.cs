using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mehspot.Core.Builders;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace Mehspot.Core.Models
{
    public delegate void GoToMessagingHandler(string userId, string userName);

    public class ViewBadgeProfileModel<TCell> : IListModel<TCell>
    {
        private volatile bool loading;
        private bool showRecommendations;
        public IBadgeProfileDTO Profile;
        private List<BadgeUserRecommendationDTO> recommendations;

        private readonly string currentUserId;
        private readonly AttributeCellFactory<TCell> cellFactory;
        private readonly IViewBadgeProfileController controller;
        private readonly Type resultType;

        private readonly CellBuilder<TCell> cellBuilder;

        private readonly List<TCell> profileDataCells = new List<TCell>();
        private readonly List<TCell> recommendationCells = new List<TCell>();

        public event Action OnRefreshing;
        public event Action OnRefreshed;
        public event Action OnWriteReviewButtonTouched;
        public event GoToMessagingHandler OnGoToMessaging;


        public IList<TCell> Cells => ShowRecommendations ? recommendationCells : profileDataCells;

        public bool ShowRecommendations
        {
            get
            {
                return showRecommendations;
            }

            private set
            {
                showRecommendations = value;
                controller?.ReloadCells();
            }
        }

        public ViewBadgeProfileModel(IViewBadgeProfileController controller, BadgeService badgeService, CellBuilder<TCell> cellBuilder)
        {
            this.cellBuilder = cellBuilder;
            this.cellFactory = new AttributeCellFactory<TCell>(badgeService, controller.BadgeId, cellBuilder);
            this.cellFactory.CellChanged += CellsSource_CellChanged;
            this.controller = controller;
            this.currentUserId = MehspotAppContext.Instance.AuthManager.AuthInfo.UserId;

            var genericParameter = typeof(IBadgeProfileValues)
                  .GetTypeInfo().Assembly.ExportedTypes
                  .FirstOrDefault(a => a.GetTypeInfo()
                  .GetCustomAttribute<ViewProfileDtoAttribute>()?.BadgeName == controller.BadgeName);
            resultType = typeof(BadgeProfileDTO<>).MakeGenericType(genericParameter);
        }

        public string GetTitle()
        {
            return (MehspotResources.ResourceManager.GetString(controller.BadgeName) ?? controller.BadgeName) + " Profile";
        }

        public async Task RefreshView()
        {
            if (loading)
                return;
            loading = true;

            OnRefreshing?.Invoke();
            this.controller.WindowTitle = GetTitle();

            Result<IBadgeProfileDTO> result = await cellFactory.BadgeService.GetBadgeProfileAsync(this.cellFactory.BadgeId, controller.UserId, resultType);
            if (result.IsSuccess)
            {
                this.Profile = result.Data;
                this.profileDataCells.Clear();
                this.profileDataCells.AddRange(await cellFactory.CreateCellsForObject(result.Data));

                this.controller.WindowTitle = $"{controller.BadgeName} {result.Data.Details.UserName}";
                this.controller.SetProfilePictureUrl(result.Data.Details.ProfilePicturePath);
                this.controller.Subdivision = result.Data.Details.SubdivisionName?.Trim();
                this.controller.Distance = Math.Round(result.Data.Details.Distance ?? 0, 2) + " miles";
                this.controller.HideFavoriteIcon = !result.Data.Details.IsFavorite;
                this.controller.Likes = $"{result.Data.Details.ReferenceCount} References / {result.Data.Details.RecommendationsCount} Recommendations";
                this.controller.FirstName = result.Data.Details.FirstName;
                this.controller.InfoLabel1 = result.Data.AdditionalInfo.InfoLabel1;
                this.controller.InfoLabel2 = result.Data.AdditionalInfo.InfoLabel2;
                LoadProfile();

            }
            else if (!result.IsNetworkIssue)
            {
                controller.ViewHelper.ShowAlert("Error", "Can not load profile data");
                return;
            }

            OnRefreshed?.Invoke();
            loading = false;
        }

        public Task<Result> ToggleBadgeUserDescriptionAsync(BadgeUserDescriptionDTO dto)
        {
            return cellFactory.BadgeService.ToggleBadgeUserDescriptionAsync(dto);
        }

        public void LoadProfile()
        {
            this.ShowRecommendations = false;
        }

        public async Task LoadRecommendations()
        {
            if (recommendationCells.Count == 0)
            {
                //controller.ViewHelper.ShowOverlay("Wait...");
                OnRefreshing?.Invoke();

                recommendationCells.Clear();
                var result = await cellFactory.BadgeService.GetBadgeRecommendationsAsync(this.controller.BadgeId, this.controller.UserId);
                if (result.IsSuccess)
                {
                    recommendationCells.AddRange(GetExtraCells());
                    bool reviewed = false;
                    if (result.Data?.Recommendations != null)
                    {
                        foreach (var item in result.Data.Recommendations)
                        {
                            if (item.FromUserId == currentUserId)
                            {
                                reviewed = true;
                            }

                            recommendationCells.Add(cellFactory.CreateRecommendationCell(item));
                        }

                        this.recommendations = result.Data.Recommendations;
                    }

                    if (!reviewed)
                    {
                        var createRecommendationCell = cellFactory.CreateButtonCell("Write Recommendation");
                        createRecommendationCell.OnButtonTouched += a => OnWriteReviewButtonTouched?.Invoke();
                        recommendationCells.Insert(0, (TCell)createRecommendationCell);
                    }
                }

                //controller.ViewHelper.HideOverlay();
                OnRefreshed?.Invoke();
            }

            this.ShowRecommendations = true;

            //TableView.Hidden = true;
            //TableView.Source = recommendationsDataSource;
            //TableView.ReloadData();
            //TableView.Hidden = false;
        }

        private IEnumerable<TCell> GetExtraCells()
        {
            if (this.Profile is BadgeProfileDTO<FriendshipProfileDTO>)
            {
                yield break;
            }


            var isHiredLabel = (this.Profile is BadgeProfileDTO<TennisProfileDTO> ||
                                   this.Profile is BadgeProfileDTO<GolfProfileDTO> ||
                                   this.Profile is BadgeProfileDTO<KidsPlayDateProfileDTO>) ? MehspotResources.PlayedBefore : MehspotResources.HiredBefore;

            yield return (TCell)this.cellBuilder
                                    .GetBooleanCell(this.Profile.Details.IsHired, (v) =>
                                 {
                                     this.Profile.Details.IsHired = v;
                                     CellsSource_CellChanged(this.Profile.Details, nameof(BadgeProfileDetailsDTO.IsHired), v);
                                 }, isHiredLabel);
            yield return (TCell)this.cellBuilder
                                    .GetBooleanCell(this.Profile.Details.HasReference, (v) =>
            {
                this.Profile.Details.IsHired = v;
                CellsSource_CellChanged(this.Profile.Details, nameof(BadgeProfileDetailsDTO.HasReference), v);
            }, "Add Reference");

            yield return (TCell)this.cellBuilder
                                    .GetTextViewCell(this.Profile.Details.ReferenceCount.ToString(), MehspotResources.ReferencesCount);
        }

        public async Task ToggleFavoriteAsync()
        {
            var dto = new BadgeUserDescriptionDTO
            {
                BadgeId = this.controller.BadgeId,
                BadgeName = this.controller.BadgeName,
                Delete = this.Profile.Details.IsFavorite,
                EmployeeId = this.controller.UserId,
                Type = BadgeDescriptionTypeEnum.Favourite
            };

            this.controller.HideFavoriteIcon = this.Profile.Details.IsFavorite;
            var result = await ToggleBadgeUserDescriptionAsync(dto);
            if (result.IsSuccess)

            {
                this.Profile.Details.IsFavorite = !this.Profile.Details.IsFavorite;
            }
            else
            {
                this.controller.HideFavoriteIcon = !this.Profile.Details.IsFavorite;
            }
        }

        public void AddRecommendation(BadgeUserRecommendationDTO recommendation)
        {
            recommendationCells.Add(cellFactory.CreateRecommendationCell(recommendation));
        }

        public void HideCreateButton()
        {
            recommendationCells.RemoveAll(a => a is IButtonCell);
        }

        private void CellsSource_CellChanged(object obj, string propertyName, object value)
        {
            if (obj is BadgeProfileDetailsDTO)
            {
                if (propertyName == nameof(BadgeProfileDetailsDTO.IsHired))
                {
                    cellFactory.BadgeService.ToggleBadgeEmploymentHistoryAsync(Profile.Details.UserId, this.cellFactory.BadgeId, !(bool)value);
                }
                else if (propertyName == nameof(BadgeProfileDetailsDTO.HasReference))
                {
                    var dto = new BadgeUserDescriptionDTO
                    {
                        BadgeId = controller.BadgeId,
                        EmployeeId = Profile.Details.UserId,
                        BadgeName = controller.BadgeName,
                        Delete = !(bool)value,
                        Type = BadgeDescriptionTypeEnum.Reference
                    };

                    ToggleBadgeUserDescriptionAsync(dto);
                }
            }
        }

        public void RowSelected(int row)
        {
            if (ShowRecommendations && row >=4)
            {
                var dto = recommendations[row-4];
                if (this.currentUserId != dto.FromUserId)
                {
                    OnGoToMessaging?.Invoke(dto.FromUserId, dto.FromUserName);
                }
            }
        }
    }
}



