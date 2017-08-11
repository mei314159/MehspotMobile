using System.Collections.Generic;
using System.Linq;
using Mehspot.Core.DTO;

namespace Mehspot.Core.Services.Badges
{
    public class BadgeHelper
    {
        public ICollection<BadgeSummaryDTO> Items { get; set; }

        public BadgeSummaryDTO GetBadgeSummary(string badgeName)
        {
            return Items.FirstOrDefault(a => a.BadgeName == badgeName);
        }

        public IReadOnlyDictionary<BadgeGroup, IReadOnlyCollection<BadgeInfo>> GetGroups()
        {
            var groups = new Dictionary<BadgeGroup, IReadOnlyCollection<BadgeInfo>>();
            groups.Add(BadgeGroup.Friends, new List<BadgeInfo>{
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

            groups.Add(BadgeGroup.Helpers, new List<BadgeInfo>{
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

            groups.Add(BadgeGroup.Jobs, new List<BadgeInfo>{
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

            //TODO: THIS IS TEMPORARY SOLUTION
            foreach (var group in groups)
            {
                foreach (var item in group.Value)
                {
                    item.Badge = Items.FirstOrDefault(a => a.BadgeName == item.BadgeName);
                }
            }

            return groups;
        }
    }

    public class BadgeInfo
    {
        public string BadgeName { get; set; }
        public string SearchBadge { get; set; }
        public string CustomKey { get; set; }
        public string CustomDescription { get; set; }
        public BadgeSummaryDTO Badge { get; set; }
    }
}
