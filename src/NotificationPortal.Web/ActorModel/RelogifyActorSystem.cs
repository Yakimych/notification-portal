using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationPortal.Web.ActorModel
{
    public record RelogifyActorModel
    {
        public ActorSystem ActorSystem { get; init; }
        public IActorRef ChallengeCreationActor { get; init; }
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
            system.EventStream.Subscribe(challengeUpdateActor, typeof(FirebaseNotificationSentMessage));
            system.EventStream.Subscribe(challengeUpdateActor, typeof(FirebaseResponseNotificationSentMessage));
            system.EventStream.Subscribe(challengeUpdateActor, typeof(ChallengeAcceptedMessage));
            system.EventStream.Subscribe(challengeUpdateActor, typeof(ChallengeDeclinedMessage));

            var notificationCreationActor = system.ActorOf<NotificationCreationActor>("notification-creation-actor");
            system.EventStream.Subscribe(notificationCreationActor, typeof(FirebaseNotificationSentMessage));
            system.EventStream.Subscribe(notificationCreationActor, typeof(FirebaseResponseNotificationSentMessage));

            return new RelogifyActorModel { ActorSystem = system, ChallengeCreationActor = challengeCreationActor };
        }
    }
}
