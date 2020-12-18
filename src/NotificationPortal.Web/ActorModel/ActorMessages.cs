using NotificationPortal.Data;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.ActorModel
{
    public record ChallengeIssuedMessage
    {
        public SendChallengeModel SendChallengeModel { get; init; }
    }

    public record ChallengeEntrySavedMessage
    {
        public ChallengeEntry ChallengeEntry { get; init; }
    }

    public record FirebaseNotificationSentMessage
    {
        public ChallengeEntry ChallengeEntry { get; init; }
        public ChallengeNotification ChallengeNotification { get; init; }
    }

    public record ChallengeStatusUpdatedMessage
    {
        public int ChallengeEntryId { get; init; }
        public ChallengeStatus NewStatus { get; init; }
    }
}
