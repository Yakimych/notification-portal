using NotificationPortal.Data;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.ActorModel
{
    public record ChallengeIssuedMessage
    {
        public SendChallengeModel SendChallengeModel { get; init; }
    }

    public record ChallengeAcceptedMessage
    {
        public int ChallengeEntryId { get; init; }
    }

    public record ChallengeDeclinedMessage
    {
        public int ChallengeEntryId { get; init; }
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

    public record FirebaseResponseNotificationSentMessage
    {
        // TODO: Is it enough to use the Id here?
        public ChallengeEntry ChallengeEntry { get; init; }
        public ChallengeNotification ChallengeNotification { get; init; }
    }

    public record ChallengeStatusUpdatedMessage
    {
        public ChallengeEntry ChallengeEntry { get; init; }
        public ChallengeStatus NewStatus { get; init; }
    }
}
