using System;
using System.Threading.Tasks;
using Akka.Actor;
using NotificationPortal.Data;

namespace NotificationPortal.Web.ActorModel
{
    public class ChallengeCreationActor : ReceiveActor
    {
        private async Task<ChallengeEntry> FakeSaveToDb()
        {
            await Task.Delay(1000);
            return new ChallengeEntry
            {
                Id = 1,
                CommunityName = "com",
                FromPlayer = "p1",
                ToPlayer = "p2",
                Status = ChallengeStatus.Challenging,
                Date = DateTime.UtcNow
            };
        }

        public ChallengeCreationActor()
        {
            Receive<ChallengeIssuedMessage>(message =>
            {
                Console.WriteLine($"{nameof(ChallengeCreationActor)}: received message: {message}");
                Console.WriteLine($"Adding challenge to the database");

                var eventStream = Context.System.EventStream;
                FakeSaveToDb().ContinueWith(saveChallengeTask => eventStream.Publish(new ChallengeEntrySavedMessage
                    { ChallengeEntry = saveChallengeTask.Result }));
            });
        }
    }

    public class SignalRActor : ReceiveActor
    {
        public SignalRActor()
        {
            Receive<ChallengeEntrySavedMessage>(message =>
            {
                Console.WriteLine($"{nameof(SignalRActor)}: challenge entry saved: {message}");
            });

            Receive<ChallengeStatusUpdatedMessage>(message =>
            {
                Console.WriteLine($"{nameof(SignalRActor)}: challenge status updated: {message}");
            });
        }
    }

    public record RelogifyActorModel
    {
        public ActorSystem ActorSystem { get; init; }
        public IActorRef ChallengeCreationActor { get; init; }
        public IActorRef SignalRActor { get; init; }
    }

    public static class RelogifyActorSystem
    {
        public static RelogifyActorModel CreateModel()
        {
            ActorSystem system = ActorSystem.Create("relogify-actor-system");

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
