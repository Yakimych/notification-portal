using Akka.Actor;
using NotificationPortal.Data;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationPortal.Web.ActorModel
{
    public class NotificationCreationActor : ReceiveActor
    {
        private void SaveNotification(int challengeEntryId, ChallengeNotification challengeNotification)
        {
            using var serviceScope = Context.CreateScope();
            var notificationPersistence = serviceScope.ServiceProvider.GetService<NotificationPersistence>();

            // TODO: Make sure an error is correctly handled if NotificationPersistence is not registered
            notificationPersistence?.AddNotification(challengeEntryId, challengeNotification);
        }

        public NotificationCreationActor()
        {
            Receive<FirebaseInitialChallengeNotificationSentMessage>(message =>
                SaveNotification(message.ChallengeEntry.Id, message.ChallengeNotification));

            Receive<FirebaseChallengeResponseNotificationSentMessage>(message =>
                SaveNotification(message.ChallengeEntry.Id, message.ChallengeNotification));
        }
    }
}
