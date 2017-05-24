using System;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;

namespace Mehspot.Core
{
    public interface ISearchFilterController
    {
        IViewHelper ViewHelper { get; }

        BadgeSummaryDTO BadgeSummary { get; }

        void ReloadData();
    }
}
