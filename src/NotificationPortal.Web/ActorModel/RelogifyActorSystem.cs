using System;
using Akka.Actor;

namespace NotificationPortal.Web.ActorModel
{
    public class ChallengeCreationActor : ReceiveActor
    {
        public ChallengeCreationActor()
        {
            Receive<ChallengeIssuedMessage>(message =>
            {
                Console.WriteLine($"received message: {message}");
            });
        }
    }

    public record RelogifyActorModel
    {
        public ActorSystem ActorSystem { get; init; }
        public IActorRef ChallengeCreationActor { get; init; }
    }

    public static class RelogifyActorSystem
    {
        public static RelogifyActorModel CreateModel()
        {
            ActorSystem system = ActorSystem.Create("relogify-actor-system");

            var challengeCreationActor = system.ActorOf<ChallengeCreationActor>();

            system.EventStream.Subscribe(challengeCreationActor, typeof(ChallengeIssuedMessage));

            return new RelogifyActorModel { ActorSystem = system, ChallengeCreationActor = challengeCreationActor };
        }
    }
}
