using NotificationPortal.Data;

namespace NotificationPortal.Web.Models
{
    public static class MappingExtensions
    {
        public static ChallengeModel ToChallengeModel(this ChallengeEntry challengeEntry)
        {
            return new(
                Id: challengeEntry.Id,
                CommunityName: challengeEntry.CommunityName,
                FromPlayer: challengeEntry.FromPlayer,
                ToPlayer: challengeEntry.ToPlayer,
                Status: challengeEntry.Status,
                Date: challengeEntry.Date);
        }
    }
}
