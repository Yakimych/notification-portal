using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationPortal.Web.ActorModel
{
    public class RelogifyActorModel
    {
        private readonly ActorSystem _actorSystem;

        public RelogifyActorModel(ActorSystem actorSystem)
        {
            _actorSystem = actorSystem;
        }

        public void PublishMessage(object message) => _actorSystem.EventStream.Publish(message);
    }

    public static class RelogifyActorSystem
    {
        public static RelogifyActorModel CreateModel(IServiceScopeFactory serviceScopeFactory)
        {
            var system = ActorSystem.Create("relogify-actor-system");
            system.AddServiceScopeFactory(serviceScopeFactory);

            var challengeCreationActor = system.ActorOf<ChallengeCreationActor>("challenge-creation-actor");
            system.EventStream.Subscribe(challengeCreationActor, typeof(ChallengeIssuedMessage));

            var firebaseActor = system.ActorOf<FirebaseActor>("firebase-actor");
            system.EventStream.Subscribe(firebaseActor, typeof(ChallengeEntrySavedMessage));
            system.EventStream.Subscribe(firebaseActor, typeof(ChallengeStatusUpdatedMessage));

            var signalRActor = system.ActorOf<SignalRActor>("signalr-actor");
            system.EventStream.Subscribe(signalRActor, typeof(ChallengeEntrySavedMessage));
            system.EventStream.Subscribe(signalRActor, typeof(ChallengeStatusUpdatedMessage));

            var challengeUpdateActor = system.ActorOf<ChallengeUpdateActor>("challenge-update-actor");
            system.EventStream.Subscribe(challengeUpdateActor, typeof(FirebaseInitialChallengeNotificationSentMessage));
            system.EventStream.Subscribe(challengeUpdateActor,
                typeof(FirebaseChallengeResponseNotificationSentMessage));
            system.EventStream.Subscribe(challengeUpdateActor, typeof(ChallengeAcceptedMessage));
            system.EventStream.Subscribe(challengeUpdateActor, typeof(ChallengeDeclinedMessage));

            var notificationCreationActor = system.ActorOf<NotificationCreationActor>("notification-creation-actor");
            system.EventStream.Subscribe(notificationCreationActor,
                typeof(FirebaseInitialChallengeNotificationSentMessage));
            system.EventStream.Subscribe(notificationCreationActor,
                typeof(FirebaseChallengeResponseNotificationSentMessage));

            return new RelogifyActorModel(actorSystem: system);
        }
    }
}
