using System;
using System.Collections.Immutable;
using NotificationPortal.Data;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.ActorModel
{
    public record ChallengeIssuedMessage(SendChallengeModel SendChallengeModel, DateTime TimeStamp);

    public record ChallengeAcceptedMessage(int ChallengeEntryId, DateTime TimeStamp);

    public record ChallengeDeclinedMessage(int ChallengeEntryId, DateTime TimeStamp);

    public record GetChallengesMessage
    {
    }

    public record GetChallengesResponse(ImmutableList<ChallengeEntry> ChallengeEntries);

    public record GetNotificationsMessage
    {
    }

    public record GetNotificationsResponse(ImmutableList<ChallengeNotification> Notifications);

    public record ChallengeEntrySavedMessage(ChallengeEntry ChallengeEntry);

    public record FirebaseInitialChallengeNotificationSentMessage(
        ChallengeEntry ChallengeEntry, ChallengeNotification ChallengeNotification);

    public record FirebaseChallengeResponseNotificationSentMessage(
        ChallengeEntry ChallengeEntry, ChallengeNotification ChallengeNotification);

    public record ChallengeStatusUpdatedMessage(ChallengeEntry ChallengeEntry, ChallengeStatus NewStatus);
}
