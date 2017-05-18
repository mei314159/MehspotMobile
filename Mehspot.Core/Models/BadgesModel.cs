using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.DTO;
using Mehspot.Core.Services;

namespace Mehspot.Core.Models
{

    public class BadgesModel
    {
        private volatile bool loading;

        private readonly BadgeService badgesService;
        private readonly IBadgesViewController viewController;

        public BadgeSummaryDTO[] Items;
        private int selectedBadgeIndex;
        private List<int> expandedRows = new List<int>();


        public event Action LoadingStart;
        public event Action LoadingEnd;

        public BadgesModel(BadgeService messagesService, IBadgesViewController viewController)
        {
            this.viewController = viewController;
            this.badgesService = messagesService;
        }

        public int Page { get; private set; } = 1;
        public BadgeSummaryDTO SelectedBadge => Items?[selectedBadgeIndex];

        public async Task RefreshAsync()
        {
            if (loading)
                return;
            loading = true;
            LoadingStart?.Invoke();
            var result = await badgesService.GetBadgesSummaryAsync();
            if (result.IsSuccess)
            {
                this.Items = result.Data;
                viewController.DisplayBadges();
            }
            else
            {
                viewController.ViewHelper.ShowAlert("Error", "Can not load badges");
            }

            LoadingEnd?.Invoke();
            loading = false;
        }

        public void SelectBadge(BadgeSummaryDTO dto)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == dto) {
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
    }
}
