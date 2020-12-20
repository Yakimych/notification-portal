using System;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using NotificationPortal.Data;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationPortal.Web.ActorModel
{
    public class ChallengeUpdateActor : ReceiveActor
    {
        private async Task<ChallengeEntry> UpdateStatusInDb(
            int challengeEntryId, ChallengeStatus newStatus, ApplicationDbContext dbContext)
        {
            var challengeToUpdate = await dbContext.ChallengeEntries.FindAsync(challengeEntryId);
            // if (challengeToAccept == null)
            // TODO: return OperationResult.NotFound;
            dbContext.Entry(challengeToUpdate).State = EntityState.Detached;

            // TODO: Date should be part of message payload
            var challengeToSave = challengeToUpdate with { Date = DateTime.UtcNow, Status = newStatus };
            dbContext.ChallengeEntries.Update(challengeToSave);
            await dbContext.SaveChangesAsync();

            return challengeToSave;
        }

        private void UpdateStatusForChallengeEntry(int challengeEntryId, ChallengeStatus newStatus)
        {
            using var serviceScope = Context.CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

            var eventStream = Context.System.EventStream;

            UpdateStatusInDb(challengeEntryId, newStatus, dbContext)
                .ContinueWith(updateTask =>
                    eventStream.Publish(
                        new ChallengeStatusUpdatedMessage
                            { ChallengeEntry = updateTask.Result, NewStatus = newStatus }));
        }

        public ChallengeUpdateActor()
        {
            Receive<FirebaseInitialChallengeNotificationSentMessage>(message =>
            {
                using var serviceScope = Context.CreateScope();
                using var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                var eventStream = Context.System.EventStream;

                var newStatus = ChallengeStatus.Challenged;
                UpdateStatusInDb(message.ChallengeEntry.Id, newStatus, dbContext)
                    .ContinueWith(_ =>
                        eventStream.Publish(
                            new ChallengeStatusUpdatedMessage
                                { ChallengeEntry = message.ChallengeEntry, NewStatus = newStatus }));
            });

            Receive<ChallengeAcceptedMessage>(message =>
                UpdateStatusForChallengeEntry(message.ChallengeEntryId, ChallengeStatus.Accepting));

            Receive<ChallengeDeclinedMessage>(message =>
                UpdateStatusForChallengeEntry(message.ChallengeEntryId, ChallengeStatus.Declining));

            Receive<FirebaseChallengeResponseNotificationSentMessage>(message =>
            {
                using var serviceScope = Context.CreateScope();
                using var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                var eventStream = Context.System.EventStream;

                // TODO: Conversion function that takes all NotificationTypes into account
                // Alternatively, prevent FirebaseResponseMessageSent from having NotificationType.Challenged as an option
                var newStatus = message.ChallengeNotification.Type == NotificationType.Accepted
                    ? ChallengeStatus.Accepted
                    : ChallengeStatus.Declined;

                UpdateStatusInDb(message.ChallengeEntry.Id, newStatus, dbContext)
                    .ContinueWith(_ =>
                        eventStream.Publish(
                            new ChallengeStatusUpdatedMessage
                                { ChallengeEntry = message.ChallengeEntry, NewStatus = newStatus }));
            });
        }
    }
}
