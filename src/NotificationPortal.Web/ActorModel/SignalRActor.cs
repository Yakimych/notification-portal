using System;
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
                Console.WriteLine($"{nameof(SignalRActor)}: challenge entry saved: {message}");

                using var serviceScope = Context.CreateScope();
                var challengeHubContext = serviceScope.ServiceProvider.GetService<IHubContext<ChallengeHub>>();
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
                Console.WriteLine($"{nameof(SignalRActor)}: challenge status updated: {message}");

                using var serviceScope = Context.CreateScope();
                var challengeHubContext = serviceScope.ServiceProvider.GetService<IHubContext<ChallengeHub>>();
                challengeHubContext?.Clients.All.SendAsync(
                    "ChallengeStatusChanged",
                    message.ChallengeEntryId,
                    message.NewStatus.ToString());
            });
        }
    }
}
