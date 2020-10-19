using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NotificationPortal.Web.Data;

namespace NotificationPortal.Web.Hubs
{
    public class ChallengeHub : Hub
    {
        // TODO: Can this be triggered via the strongly typed IHubContext?
        // https://docs.microsoft.com/en-us/aspnet/core/signalr/hubcontext?view=aspnetcore-3.1#inject-a-strongly-typed-hubcontext
        // TODO: Remove otherwise
        public Task BroadcastChallengeIssued(Notification notification) =>
            Clients.All.SendAsync(
                "NewChallengeIssued",
                notification.Id,
                notification.CommunityName,
                notification.FromPlayer,
                notification.ToPlayer,
                ChallengeStatus.Challenging.ToString(),
                notification.Date);

        public Task BroadcastChallengeStatusChanged(int challengeId, ChallengeStatus newStatus) =>
            Clients.All.SendAsync("ChallengeStatusChanged", challengeId, newStatus.ToString());

        // Methods used for testing
        public async Task NewChallenge()
        {
            await Clients.All.SendAsync("NewChallengeIssued", 123, "testCommunity", "fromPlayer1", "toPlayer2",
                ChallengeStatus.Challenging.ToString(), DateTime.Now.FormatDateTime());
            await Task.Delay(3000);
            await Clients.All.SendAsync("ChallengeStatusChanged", 123, ChallengeStatus.Challenged.ToString());
        }

        public async Task AcceptChallenge(int challengeId)
        {
            await Clients.All.SendAsync("ChallengeStatusChanged", challengeId, ChallengeStatus.Accepting.ToString());
            await Task.Delay(3000);
            await Clients.All.SendAsync("ChallengeStatusChanged", challengeId, ChallengeStatus.Accepted.ToString());
        }

        public async Task DeclineChallenge(int challengeId)
        {
            await Clients.All.SendAsync("ChallengeStatusChanged", challengeId, ChallengeStatus.Declining.ToString());
            await Task.Delay(3000);
            await Clients.All.SendAsync("ChallengeStatusChanged", challengeId, ChallengeStatus.Declined.ToString());
        }
    }
}
