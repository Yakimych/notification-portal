using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NotificationPortal.Web.Core;
using NotificationPortal.Data;

namespace NotificationPortal.Web.Hubs
{
    public class ChallengeHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ChallengeService _challengeService;

        // TODO: Can this be triggered via the strongly typed IHubContext?
        // https://docs.microsoft.com/en-us/aspnet/core/signalr/hubcontext?view=aspnetcore-3.1#inject-a-strongly-typed-hubcontext
        public ChallengeHub(ApplicationDbContext dbContext, ChallengeService challengeService)
        {
            _dbContext = dbContext;
            _challengeService = challengeService;
        }

        public Task AcceptChallenge(int challengeId) =>
            _challengeService.AcceptChallenge(challengeId);

        public Task DeclineChallenge(int challengeId) =>
            _challengeService.DeclineChallenge(challengeId);
    }
}
