using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationPortal.Web.ActorModel
{
    public class RelogifyActorModel
    {
        public ActorSystem ActorSystem { private get; init; }
        public IActorRef ChallengeActor { get; init; }

        public void PublishMessage(object message) => ActorSystem.EventStream.Publish(message);
    }

    public static class RelogifyActorSystem
    {
        public static RelogifyActorModel CreateModel(IServiceScopeFactory serviceScopeFactory)
        {
            var system = ActorSystem.Create("relogify-actor-system");
            system.AddServiceScopeFactory(serviceScopeFactory);

            var challengeActor = system.ActorOf<ChallengeActor>("challenge-actor");
            system.EventStream.Subscribe(challengeActor, typeof(ChallengeIssuedMessage));
            system.EventStream.Subscribe(challengeActor, typeof(ChallengeAcceptedMessage));
            system.EventStream.Subscribe(challengeActor, typeof(ChallengeDeclinedMessage));
            system.EventStream.Subscribe(challengeActor, typeof(FirebaseInitialChallengeNotificationSentMessage));
            system.EventStream.Subscribe(challengeActor, typeof(FirebaseChallengeResponseNotificationSentMessage));

            var firebaseActor = system.ActorOf<FirebaseActor>("firebase-actor");
            system.EventStream.Subscribe(firebaseActor, typeof(ChallengeEntrySavedMessage));
            system.EventStream.Subscribe(firebaseActor, typeof(ChallengeStatusUpdatedMessage));

            var signalRActor = system.ActorOf<SignalRActor>("signalr-actor");
            system.EventStream.Subscribe(signalRActor, typeof(ChallengeEntrySavedMessage));
            system.EventStream.Subscribe(signalRActor, typeof(ChallengeStatusUpdatedMessage));

            var notificationCreationActor = system.ActorOf<NotificationCreationActor>("notification-creation-actor");
            system.EventStream.Subscribe(notificationCreationActor,
                typeof(FirebaseInitialChallengeNotificationSentMessage));
            system.EventStream.Subscribe(notificationCreationActor,
                typeof(FirebaseChallengeResponseNotificationSentMessage));

            return new RelogifyActorModel { ActorSystem = system, ChallengeActor = challengeActor };
        }
    }
}
