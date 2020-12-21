using NotificationPortal.Data;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.ActorModel
{
    public record ChallengeIssuedMessage(SendChallengeModel SendChallengeModel);

    public record ChallengeAcceptedMessage(int ChallengeEntryId);

    public record ChallengeDeclinedMessage(int ChallengeEntryId);

    public record ChallengeEntrySavedMessage(ChallengeEntry ChallengeEntry);

    public record FirebaseInitialChallengeNotificationSentMessage(
        ChallengeEntry ChallengeEntry, ChallengeNotification ChallengeNotification);

    public record FirebaseChallengeResponseNotificationSentMessage(
        ChallengeEntry ChallengeEntry, ChallengeNotification ChallengeNotification);

    public record ChallengeStatusUpdatedMessage(ChallengeEntry ChallengeEntry, ChallengeStatus NewStatus);
}
