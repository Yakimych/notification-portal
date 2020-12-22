using System;
using Akka.Actor;
using NotificationPortal.Data;

namespace NotificationPortal.Web.ActorModel
{
    public class ChallengeActor : ReceiveActor
    {
        public ChallengeActor()
        {
            Receive<ChallengeIssuedMessage>(message =>
            {
                using var serviceScope = Context.CreateScope();
                var challengePersistence = ServiceScopeHelper.GetService<ChallengePersistence>(serviceScope);

                var eventStream = Context.System.EventStream;

                var newChallenge = new ChallengeEntry
                {
                    CommunityName = message.SendChallengeModel.CommunityName,
                    FromPlayer = message.SendChallengeModel.FromPlayer,
                    ToPlayer = message.SendChallengeModel.ToPlayer,
                    Status = ChallengeStatus.Challenging,
                    Date = DateTime.UtcNow
                };

                challengePersistence.AddToDb(newChallenge).ContinueWith(saveChallengeTask =>
                    eventStream.Publish(new ChallengeEntrySavedMessage(ChallengeEntry: saveChallengeTask.Result)));
            });

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

            Receive<GetChallengesMessage>(_ =>
            {
                var serviceScope = Context.CreateScope();
                var challengePersistence = ServiceScopeHelper.GetService<ChallengePersistence>(serviceScope);

                challengePersistence
                    .GetAllFromDb()
                    .ContinueWith(getChallengesTask =>
                    {
                        serviceScope.Dispose();
                        return new GetChallengesResponse(ChallengeEntries: getChallengesTask.Result);
                    })
                    .PipeTo(Sender);
            });
        }

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
    }
}
