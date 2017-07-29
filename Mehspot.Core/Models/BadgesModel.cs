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

        public BadgeSummaryDTO[] Items;
        public event Action LoadingStart;
        public event Action LoadingEnd;
        public volatile bool dataLoaded;


        public BadgesModel(BadgeService messagesService, IBadgesViewController viewController)
        {
            this.viewController = viewController;
            this.badgesService = messagesService;
            this.BadgeHelper = new BadgeHelper();
        }

        public BadgeHelper BadgeHelper { get; private set; }
        public int Page { get; private set; } = 1;
        public BadgeSummaryDTO SelectedBadge => Items?[selectedBadgeIndex];

        public bool TryLoadFromCache()
        {
            var cachedBadgeSummary = badgesService.CachedBadgeSummary;
            if (cachedBadgeSummary != null)
            {
                BadgeHelper.Items = this.Items = cachedBadgeSummary;
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
                    BadgeHelper.Items = this.Items = result.Data;
                    viewController.DisplayBadges();
                }
                else
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

        public bool IsRowExpanded(int row)
        {
            return expandedRows.Contains(row);
        }

        public void CollapseRows()
        {
            expandedRows.Clear();
        }
    }
}
