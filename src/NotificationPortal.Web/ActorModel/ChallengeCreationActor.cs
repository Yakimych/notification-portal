using System;
using System.Threading.Tasks;
using Akka.Actor;
using NotificationPortal.Data;
using Microsoft.Extensions.DependencyInjection;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.ActorModel
{
    public class ChallengeCreationActor : ReceiveActor
    {
        private async Task<ChallengeEntry> SaveToDb(
            SendChallengeModel sendChallengeModel, ApplicationDbContext dbContext)
        {
            var newChallenge = new ChallengeEntry
            {
                CommunityName = sendChallengeModel.CommunityName,
                FromPlayer = sendChallengeModel.FromPlayer,
                ToPlayer = sendChallengeModel.ToPlayer,
                Status = ChallengeStatus.Challenging,
                Date = DateTime.UtcNow
            };

            await dbContext.ChallengeEntries.AddAsync(newChallenge);
            await dbContext.SaveChangesAsync();

            return newChallenge;
        }

        public ChallengeCreationActor()
        {
            Receive<ChallengeIssuedMessage>(message =>
            {
                using var serviceScope = Context.CreateScope();
                using var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                var eventStream = Context.System.EventStream;
                SaveToDb(message.SendChallengeModel, dbContext)
                    .ContinueWith(saveChallengeTask =>
                        eventStream.Publish(
                            new ChallengeEntrySavedMessage { ChallengeEntry = saveChallengeTask.Result }));
            });
        }
    }
}
