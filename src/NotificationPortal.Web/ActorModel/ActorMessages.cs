using System.Collections.Immutable;
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

    public record GetChallengesMessage { }

    public record GetChallengesResponse
    {
        public ImmutableList<ChallengeEntry> ChallengeEntries { get; init; }
    }

    public record GetNotificationsMessage { }

    public record GetNotificationsResponse
    {
        public ImmutableList<ChallengeNotification> Notifications { get; init; }
    }

    public record ChallengeEntrySavedMessage
    {
        public ChallengeEntry ChallengeEntry { get; init; }
    }

    public record FirebaseInitialChallengeNotificationSentMessage
    {
        public ChallengeEntry ChallengeEntry { get; init; }
        public ChallengeNotification ChallengeNotification { get; init; }
    }

    public record FirebaseChallengeResponseNotificationSentMessage
    {
        public ChallengeEntry ChallengeEntry { get; init; }
        public ChallengeNotification ChallengeNotification { get; init; }
    }

    public record ChallengeStatusUpdatedMessage
    {
        public ChallengeEntry ChallengeEntry { get; init; }
        public ChallengeStatus NewStatus { get; init; }
    }
}
