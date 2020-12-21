using System;
using Akka.Actor;
using NotificationPortal.Data;

namespace NotificationPortal.Web.ActorModel
{
    public class ChallengeUpdateActor : ReceiveActor
    {
        private void UpdateStatusForChallengeEntry(int challengeEntryId, ChallengeStatus newStatus, DateTime timestamp)
        {
            using var serviceScope = Context.CreateScope();
            var challengePersistence = ServiceScopeHelper.GetService<ChallengePersistence>(serviceScope);

            var eventStream = Context.System.EventStream;

            challengePersistence.UpdateStatusInDb(challengeEntryId, newStatus, timestamp)
                .ContinueWith(updateTask =>
                    eventStream.Publish(
                        new ChallengeStatusUpdatedMessage
                            (ChallengeEntry: updateTask.Result, NewStatus: newStatus)));
        }

        public ChallengeUpdateActor()
        {
            Receive<FirebaseInitialChallengeNotificationSentMessage>(message =>
                UpdateStatusForChallengeEntry(message.ChallengeEntry.Id, ChallengeStatus.Challenged, DateTime.UtcNow));

            Receive<ChallengeAcceptedMessage>(message =>
                UpdateStatusForChallengeEntry(message.ChallengeEntryId, ChallengeStatus.Accepting, DateTime.UtcNow));

            Receive<ChallengeDeclinedMessage>(message =>
                UpdateStatusForChallengeEntry(message.ChallengeEntryId, ChallengeStatus.Declining, DateTime.UtcNow));

            Receive<FirebaseChallengeResponseNotificationSentMessage>(message =>
            {
                var newStatus = message.ChallengeNotification.Type == NotificationType.Accepted
                    ? ChallengeStatus.Accepted
                    : ChallengeStatus.Declined;
                UpdateStatusForChallengeEntry(message.ChallengeEntry.Id, newStatus, DateTime.UtcNow);
            });
        }
    }
}
