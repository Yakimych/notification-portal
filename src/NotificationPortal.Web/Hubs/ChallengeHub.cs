using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace NotificationPortal.Web.Hubs
{
    public class ChallengeHub : Hub
    {
        public async Task SendMessage(int challengeId, string challengeAction)
        {
            await Clients.All.SendAsync("ReceiveChallengeAction", challengeId, challengeAction);
        }

        public async Task AcceptChallenge(int challengeId)
        {
            await Clients.All.SendAsync("ChallengeAccepted", challengeId);
        }

        public async Task DeclineChallenge(int challengeId)
        {
            await Clients.All.SendAsync("ChallengeDeclined", challengeId);
        }
    }
}
