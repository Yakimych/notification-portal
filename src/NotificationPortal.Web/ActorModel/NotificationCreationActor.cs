using Akka.Actor;
using NotificationPortal.Data;

namespace NotificationPortal.Web.ActorModel
{
    public class NotificationCreationActor : ReceiveActor
    {
        private void SaveNotification(int challengeEntryId, ChallengeNotification challengeNotification)
        {
            using var serviceScope = Context.CreateScope();
            var notificationPersistence = ServiceScopeHelper.GetService<NotificationPersistence>(serviceScope);

            notificationPersistence.AddNotification(challengeEntryId, challengeNotification);
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
