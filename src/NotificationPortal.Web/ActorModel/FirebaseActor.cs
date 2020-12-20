using Akka.Actor;
using NotificationPortal.Data;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationPortal.Web.ActorModel
{
    public class FirebaseActor : ReceiveActor
    {
        private void SendResponseViaFirebaseAndPublishEvent(
            ChallengeEntry challengeEntry, NotificationType notificationType)
        {
            using var serviceScope = Context.CreateScope();
            var firebaseMessagingService = serviceScope.ServiceProvider.GetService<FirebaseMessagingService>();
            // TODO: Throw in firebaseMessagingService is not registered

            var eventStream = Context.System.EventStream;
            firebaseMessagingService?.SendMessageWithResponseToChallenge(challengeEntry, notificationType)
                .ContinueWith(sendToFirebaseTask =>
                    eventStream.Publish(
                        new FirebaseChallengeResponseNotificationSentMessage
                        {
                            ChallengeEntry = challengeEntry,
                            ChallengeNotification = sendToFirebaseTask.Result
                        }));
        }

        public FirebaseActor()
        {
            Receive<ChallengeEntrySavedMessage>(message =>
            {
                using var serviceScope = Context.CreateScope();
                var firebaseMessagingService = serviceScope.ServiceProvider.GetService<FirebaseMessagingService>();
                // TODO: Throw in firebaseMessagingService is not registered

                var eventStream = Context.System.EventStream;
                firebaseMessagingService?.SendMessageWithInitialChallenge(message.ChallengeEntry)
                    .ContinueWith(sendToFirebaseTask =>
                        eventStream.Publish(
                            new FirebaseInitialChallengeNotificationSentMessage
                            {
                                ChallengeEntry = message.ChallengeEntry,
                                ChallengeNotification = sendToFirebaseTask.Result
                            }));
            });

            Receive<ChallengeStatusUpdatedMessage>(message =>
            {
                switch (message.NewStatus)
                {
                    case ChallengeStatus.Accepting:
                        SendResponseViaFirebaseAndPublishEvent(message.ChallengeEntry, NotificationType.Accepted);
                        break;
                    case ChallengeStatus.Declining:
                        SendResponseViaFirebaseAndPublishEvent(message.ChallengeEntry, NotificationType.Declined);
                        break;
                    default:
                        // No Firebase action required for other status changes
                        return;
                }
            });
        }
    }
}
