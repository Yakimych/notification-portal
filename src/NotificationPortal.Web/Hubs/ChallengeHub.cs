using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NotificationPortal.Web.Core;
using NotificationPortal.Web.Data;

namespace NotificationPortal.Web.Hubs
{
    public class ChallengeHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ChallengeService _challengeService;

        public ChallengeHub(ApplicationDbContext dbContext, ChallengeService challengeService)
        {
            _dbContext = dbContext;
            _challengeService = challengeService;
        }

        // TODO: Can this be triggered via the strongly typed IHubContext?
        // https://docs.microsoft.com/en-us/aspnet/core/signalr/hubcontext?view=aspnetcore-3.1#inject-a-strongly-typed-hubcontext
        // TODO: Remove otherwise
        // public Task BroadcastChallengeIssued(ChallengeNotification notification) =>
        //     Clients.All.SendAsync(
        //         "NewChallengeIssued",
        //         notification.Id,
        //         notification.CommunityName,
        //         notification.FromPlayer,
        //         notification.ToPlayer,
        //         ChallengeStatus.Challenging.ToString(),
        //         notification.Date);
        //
        // public Task BroadcastChallengeStatusChanged(int challengeId, ChallengeStatus newStatus) =>
        //     Clients.All.SendAsync("ChallengeStatusChanged", challengeId, newStatus.ToString());
        //
        // // Methods used for testing
        // public async Task NewChallenge()
        // {
        //     await Clients.All.SendAsync("NewChallengeIssued", 123, "testCommunity", "fromPlayer1", "toPlayer2",
        //         ChallengeStatus.Challenging.ToString(), DateTime.Now.FormatDateTime());
        //     await Task.Delay(3000);
        //     await Clients.All.SendAsync("ChallengeStatusChanged", 123, ChallengeStatus.Challenged.ToString());
        // }

        public async Task AcceptChallenge(int challengeId)
        {
            await _challengeService.AcceptChallenge(challengeId);
            // // TODO: Duplication with the controller
            // var challengeToAccept = await _dbContext.ChallengeEntries.FindAsync(challengeId);
            // if (challengeToAccept == null)
            //     return;
            //
            // challengeToAccept.Status = ChallengeStatus.Accepting;
            // await _dbContext.SaveChangesAsync();
            //
            // await Clients.All.SendAsync("ChallengeStatusChanged", challengeId, challengeToAccept.Status.ToString());
            //
            // BackgroundJob.Enqueue(() =>
            //     _challengeService.RespondToChallenge(challengeToAccept, NotificationType.Accepted));
        }

        public async Task DeclineChallenge(int challengeId)
        {
            await _challengeService.DeclineChallenge(challengeId);
            // // TODO: Duplication with the controller
            // var challengeToDecline = await _dbContext.ChallengeEntries.FindAsync(challengeId);
            // if (challengeToDecline == null)
            //     return;
            //
            // challengeToDecline.Status = ChallengeStatus.Accepting;
            // await _dbContext.SaveChangesAsync();
            //
            // await Clients.All.SendAsync("ChallengeStatusChanged", challengeId, challengeToDecline.Status.ToString());
            //
            // BackgroundJob.Enqueue(() =>
            //     _challengeService.RespondToChallenge(challengeToDecline, NotificationType.Declined));
        }
    }
}
