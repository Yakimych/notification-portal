using Akka.Actor;
using NotificationPortal.Data;

namespace NotificationPortal.Web.ActorModel
{
    public class NotificationActor : ReceiveActor
    {
        private void SaveNotification(int challengeEntryId, ChallengeNotification challengeNotification)
        {
            using var serviceScope = Context.CreateScope();
            var notificationPersistence = ServiceScopeHelper.GetService<NotificationPersistence>(serviceScope);

            notificationPersistence.AddToDb(challengeEntryId, challengeNotification);
        }

        public NotificationActor()
        {
            Receive<FirebaseInitialChallengeNotificationSentMessage>(message =>
                SaveNotification(message.ChallengeEntry.Id, message.ChallengeNotification));

            Receive<FirebaseChallengeResponseNotificationSentMessage>(message =>
                SaveNotification(message.ChallengeEntry.Id, message.ChallengeNotification));

            Receive<GetNotificationsMessage>(_ =>
            {
                var serviceScope = Context.CreateScope();
                var notificationPersistence = ServiceScopeHelper.GetService<NotificationPersistence>(serviceScope);

                notificationPersistence
                    .GetAllFromDb()
                    .ContinueWith(getNotificationsTask =>
                    {
                        serviceScope.Dispose();
                        return new GetNotificationsResponse { Notifications = getNotificationsTask.Result };
                    })
                    .PipeTo(Sender);
            });
        }
    }
}
