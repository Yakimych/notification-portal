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
}
