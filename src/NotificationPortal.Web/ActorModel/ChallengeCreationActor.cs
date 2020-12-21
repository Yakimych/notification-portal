using System;
using Akka.Actor;
using NotificationPortal.Data;

namespace NotificationPortal.Web.ActorModel
{
    public class ChallengeCreationActor : ReceiveActor
    {
        public ChallengeCreationActor()
        {
            Receive<ChallengeIssuedMessage>(message =>
            {
                using var serviceScope = Context.CreateScope();
                var challengePersistence = ServiceScopeHelper.GetService<ChallengePersistence>(serviceScope);

                var eventStream = Context.System.EventStream;

                var newChallenge = new ChallengeEntry
                {
                    CommunityName = message.SendChallengeModel.CommunityName,
                    FromPlayer = message.SendChallengeModel.FromPlayer,
                    ToPlayer = message.SendChallengeModel.ToPlayer,
                    Status = ChallengeStatus.Challenging,
                    Date = DateTime.UtcNow
                };

                challengePersistence.SaveToDb(newChallenge).ContinueWith(saveChallengeTask =>
                    eventStream.Publish(new ChallengeEntrySavedMessage { ChallengeEntry = saveChallengeTask.Result }));
            });
        }
    }
}
