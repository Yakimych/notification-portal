using System;
using System.Threading.Tasks;
using Akka.Actor;
using NotificationPortal.Data;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationPortal.Web.ActorModel
{
    public class ChallengeUpdateActor : ReceiveActor
    {
        private async Task UpdateStatus(
            ChallengeEntry challengeEntry, ChallengeStatus newStatus, ApplicationDbContext dbContext)
        {
            var challengeToSave =
                challengeEntry with { Date = DateTime.UtcNow, Status = newStatus };
            dbContext.ChallengeEntries.Update(challengeToSave);
            await dbContext.SaveChangesAsync();
        }

        public ChallengeUpdateActor()
        {
            Receive<FirebaseNotificationSentMessage>(message =>
            {
                using var serviceScope = Context.CreateScope();
                using var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                var eventStream = Context.System.EventStream;

                var newStatus = ChallengeStatus.Challenged;
                UpdateStatus(message.ChallengeEntry, newStatus, dbContext)
                    .ContinueWith(_ =>
                        eventStream.Publish(
                            new ChallengeStatusUpdatedMessage
                                { ChallengeEntryId = message.ChallengeEntry.Id, NewStatus = newStatus }));
            });
        }
    }
}
