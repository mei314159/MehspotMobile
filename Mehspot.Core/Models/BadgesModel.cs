using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.DTO;
using Mehspot.Core.Services;
using Mehspot.Core.Services.Badges;

namespace Mehspot.Core.Models
{

    public class BadgesModel
    {
        private volatile bool loading;
        private volatile int selectedBadgeIndex;
        private readonly BadgeService badgesService;
        private readonly IBadgesViewController viewController;
        private readonly List<int> expandedRows = new List<int>();
        private BadgeSummaryDTO[] items;
		private Dictionary<BadgeGroup, IReadOnlyCollection<BadgeInfo>> groups;

        public event Action LoadingStart;
        public event Action LoadingEnd;
        public volatile bool dataLoaded;

        public BadgesModel(BadgeService messagesService, IBadgesViewController viewController)
        {
            this.viewController = viewController;
            this.badgesService = messagesService;
        }

        public int Page { get; private set; } = 1;
        public BadgeSummaryDTO SelectedBadge => Items?[selectedBadgeIndex];

        public Dictionary<BadgeGroup, IReadOnlyCollection<BadgeInfo>> Groups
        {
            get
            {
                if (groups == null)
                {
                    InitializeGroups();
                }

                return groups;
            }
        }

		public BadgeSummaryDTO[] Items
		{
			get
			{
				return items;
			}

			private set
			{
				items = value;
				//TODO: THIS IS TEMPORARY SOLUTION
				foreach (var group in this.Groups)
				{
					foreach (var item in group.Value)
					{
						item.Badge = items.FirstOrDefault(a => a.BadgeName == item.BadgeName);
					}
				}
			}
		}

		public bool TryLoadFromCache()
        {
            var cachedBadgeSummary = badgesService.CachedBadgeSummary;
            if (cachedBadgeSummary != null)
            {
                this.Items = cachedBadgeSummary;
                viewController.DisplayBadges();
                return true;
            }

            return false;
        }

        public async Task<bool> RefreshAsync(bool loadFromServer, bool showSpinner = false)
        {
            var status = false;
            if (loading)
                return status;
            loading = true;

            status = TryLoadFromCache();

            if ((showSpinner || !status) && loadFromServer)
            {
                LoadingStart?.Invoke();
            }

            if (loadFromServer || !status)
            {
                var result = await badgesService.GetBadgesSummaryAsync();
                status = result.IsSuccess;
                if (result.IsSuccess)
                {
                    this.Items = result.Data;

                    viewController.DisplayBadges();
                }
                else if (!result.IsNetworkIssue)
                {
                    viewController.ViewHelper.ShowAlert("Error", "Can not load badges");
                }

                LoadingEnd?.Invoke();
                dataLoaded = result.IsSuccess;
            }

            loading = false;
            return status;
        }

        public void SelectBadge(BadgeSummaryDTO dto)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == dto)
                {
                    SelectRow(i);
                    break;
                }
            }
        }

        public void SelectRow(int row)
        {
            this.selectedBadgeIndex = row;
            if (expandedRows.Contains(row))
            {
                expandedRows.Remove(row);
            }
            else
            {
                expandedRows.Add(row);
            }
        }

        public BadgeGroup GetCurrentKey(IReadOnlyDictionary<BadgeGroup, IReadOnlyCollection<BadgeInfo>> groups)
        {
            var preferredBadgeGroup = MehspotAppContext.Instance.DataStorage.PreferredBadgeGroup;
            if (preferredBadgeGroup == null)
            {
                if (groups[BadgeGroup.Friends].Any(a => a.Badge.IsRegistered))
                {
                    preferredBadgeGroup = BadgeGroup.Friends;
                }
                else if (groups[BadgeGroup.Helpers].Any(a => a.Badge.IsRegistered))
                {
                    preferredBadgeGroup = BadgeGroup.Helpers;
                }
                else if (groups[BadgeGroup.Jobs].Any(a => a.Badge.IsRegistered))
                {
                    preferredBadgeGroup = BadgeGroup.Jobs;
                }
                else
                {
                    preferredBadgeGroup = BadgeGroup.Friends;
                }

                MehspotAppContext.Instance.DataStorage.PreferredBadgeGroup = preferredBadgeGroup;
            }

            return preferredBadgeGroup.Value;
        }

        public bool IsRowExpanded(int row)
        {
            return expandedRows.Contains(row);
        }

        public void CollapseRows()
        {
            expandedRows.Clear();
        }

        public BadgeSummaryDTO GetBadgeSummary(string badgeName)
        {
            return Items.FirstOrDefault(a => a.BadgeName == badgeName);
        }

        private void InitializeGroups()
        {
            this.groups = new Dictionary<BadgeGroup, IReadOnlyCollection<BadgeInfo>>();
            this.groups.Add(BadgeGroup.Friends, new List<BadgeInfo>{
                new BadgeInfo
                {
                    BadgeName = Constants.BadgeNames.Friendship,
                    SearchBadge = Constants.BadgeNames.Friendship
                },
                new BadgeInfo
                {
                    BadgeName = Constants.BadgeNames.KidsPlayDate,
                    SearchBadge = Constants.BadgeNames.KidsPlayDate
                },
                new BadgeInfo
                {
                    BadgeName = Constants.BadgeNames.Tennis,
                    SearchBadge = Constants.BadgeNames.Tennis
                },
                new BadgeInfo
                {
                    BadgeName = Constants.BadgeNames.Golf,
                    SearchBadge = Constants.BadgeNames.Golf
                },
                new BadgeInfo
                {
                    BadgeName = Constants.BadgeNames.Fitness,
                    SearchBadge = Constants.BadgeNames.Fitness
                }
            });

            this.groups.Add(BadgeGroup.Helpers, new List<BadgeInfo>{
                new BadgeInfo
                {
                    BadgeName = Constants.BadgeNames.BabysitterEmployer,
                    SearchBadge = Constants.BadgeNames.Babysitter
                },
                new BadgeInfo
                {
                    BadgeName = Constants.BadgeNames.PetSitterEmployer,
                    SearchBadge = Constants.BadgeNames.PetSitter
                },
                new BadgeInfo
                {
                    BadgeName = Constants.BadgeNames.TutorEmployer,
                    SearchBadge = Constants.BadgeNames.Tutor
                },

                new BadgeInfo
                {
                    BadgeName = Constants.BadgeNames.OtherJobs,
                    SearchBadge = Constants.BadgeNames.OtherJobs,
                    CustomKey = "Helpers",
                    CustomDescription = MehspotResources.OtherHelpersDescription
                }
            });

            this.groups.Add(BadgeGroup.Jobs, new List<BadgeInfo>{
                new BadgeInfo
                {
                    BadgeName = Constants.BadgeNames.Babysitter,
                    SearchBadge = Constants.BadgeNames.BabysitterEmployer
                },
                new BadgeInfo
                {
                    BadgeName = Constants.BadgeNames.PetSitter,
                    SearchBadge = Constants.BadgeNames.PetSitterEmployer
                },
                new BadgeInfo
                {
                    BadgeName = Constants.BadgeNames.Tutor,
                    SearchBadge = Constants.BadgeNames.TutorEmployer
                },
                new BadgeInfo
                {
                    BadgeName = Constants.BadgeNames.OtherJobs,
                    SearchBadge = Constants.BadgeNames.OtherJobs
                }
            });
        }
    }
}
