using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using NotificationPortal.Web.Hubs;

namespace NotificationPortal.Web.ActorModel
{
    public class SignalRActor : ReceiveActor
    {
        public SignalRActor()
        {
            Receive<ChallengeEntrySavedMessage>(message =>
            {
                using var serviceScope = Context.CreateScope();
                var challengeHubContext = serviceScope.ServiceProvider.GetService<IHubContext<ChallengeHub>>();

                // TODO: Throw if challengeHubContext is not registered
                challengeHubContext?.Clients.All.SendAsync(
                    "NewChallengeIssued",
                    message.ChallengeEntry.Id,
                    message.ChallengeEntry.CommunityName,
                    message.ChallengeEntry.FromPlayer,
                    message.ChallengeEntry.ToPlayer,
                    message.ChallengeEntry.Status.ToString(),
                    message.ChallengeEntry.Date.FormatDateTime());
            });

            Receive<ChallengeStatusUpdatedMessage>(message =>
            {
                using var serviceScope = Context.CreateScope();
                var challengeHubContext = serviceScope.ServiceProvider.GetService<IHubContext<ChallengeHub>>();

                // TODO: Throw if challengeHubContext is not registered
                challengeHubContext?.Clients.All.SendAsync(
                    "ChallengeStatusChanged",
                    message.ChallengeEntry.Id,
                    message.NewStatus.ToString());
            });
        }
    }
}
