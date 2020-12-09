using NotificationPortal.Web.Data;

namespace NotificationPortal.Web.Models
{
    public static class MappingExtensions
    {
        public static ChallengeModel ToChallengeModel(this ChallengeEntry challengeEntry)
        {
            return new()
            {
                Id = challengeEntry.Id,
                CommunityName = challengeEntry.CommunityName,
                FromPlayer = challengeEntry.FromPlayer,
                ToPlayer = challengeEntry.ToPlayer,
                Type = challengeEntry.Status.ToString(),
                Date = challengeEntry.Date
            };
        }
    }
}
