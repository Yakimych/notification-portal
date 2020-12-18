using System.Threading.Tasks;
using Akka.Actor;
using NotificationPortal.Data;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationPortal.Web.ActorModel
{
    public class NotificationCreationActor : ReceiveActor
    {
        private async Task SaveToDb(
            int challengeEntryId, ChallengeNotification challengeNotification, ApplicationDbContext dbContext)
        {
            var challengeNotificationToSave = challengeNotification with { ChallengeEntryId = challengeEntryId };

            await dbContext.Notifications.AddAsync(challengeNotificationToSave);
            await dbContext.SaveChangesAsync();
        }

        public NotificationCreationActor()
        {
            Receive<FirebaseNotificationSentMessage>(message =>
            {
                using var serviceScope = Context.CreateScope();
                using var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                _ = SaveToDb(message.ChallengeEntry.Id, message.ChallengeNotification, dbContext);
            });
        }
    }
}
