using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationPortal.Web.ActorModel
{
    public record RelogifyActorModel
    {
        public ActorSystem ActorSystem { get; init; }
        public IActorRef ChallengeCreationActor { get; init; }
        public IActorRef SignalRActor { get; init; }
    }

    public static class RelogifyActorSystem
    {
        public static RelogifyActorModel CreateModel(IServiceScopeFactory serviceScopeFactory)
        {
            ActorSystem system = ActorSystem.Create("relogify-actor-system");
            system.AddServiceScopeFactory(serviceScopeFactory);

            var challengeCreationActor = system.ActorOf<ChallengeCreationActor>();
            var signalRActor = system.ActorOf<SignalRActor>();

            system.EventStream.Subscribe(challengeCreationActor, typeof(ChallengeIssuedMessage));
            system.EventStream.Subscribe(signalRActor, typeof(ChallengeEntrySavedMessage));
            system.EventStream.Subscribe(signalRActor, typeof(ChallengeStatusUpdatedMessage));

            return new RelogifyActorModel
                { ActorSystem = system, ChallengeCreationActor = challengeCreationActor, SignalRActor = signalRActor };
        }
    }
}
