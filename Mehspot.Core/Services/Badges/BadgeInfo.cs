using System.Collections.Generic;
using System.Linq;
using Mehspot.Core.DTO;

namespace Mehspot.Core.Services.Badges
{
    public class BadgeInfo
    {
        public string BadgeName { get; set; }
        public string SearchBadge { get; set; }
        public string CustomKey { get; set; }
        public string CustomDescription { get; set; }
        public BadgeSummaryDTO Badge { get; set; }
    }
}
