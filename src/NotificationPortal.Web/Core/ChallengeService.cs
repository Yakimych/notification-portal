using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NotificationPortal.Web.Data;
using NotificationPortal.Web.Hubs;

namespace NotificationPortal.Web.Core
{
    public class ChallengeService
    {
        private readonly FirebaseMessagingService _firebaseMessagingService;
        private readonly IHubContext<ChallengeHub> _challengeHubContext;
        private readonly ApplicationDbContext _dbContext;

        public ChallengeService(
            FirebaseMessagingService firebaseMessagingService, ApplicationDbContext dbContext,
            IHubContext<ChallengeHub> challengeHubContext)
        {
            _firebaseMessagingService = firebaseMessagingService;
            _dbContext = dbContext;
            _challengeHubContext = challengeHubContext;
        }

        public async Task InitiateChallenge(int notificationId, string communityName, string fromPlayer, string toPlayer)
        {
            // TODO: Is it possible to handle errors here? Test with spaces in player names
            await _firebaseMessagingService.SendMessage(communityName, fromPlayer, toPlayer);

            var notificationToUpdate = await _dbContext.Notifications.FindAsync(notificationId);
            notificationToUpdate.Status = ChallengeStatus.Challenged;
            await _dbContext.SaveChangesAsync();

            await _challengeHubContext.Clients.All.SendAsync("ChallengeStatusChanged", notificationId,
                ChallengeStatus.Challenged.ToString());
        }
    }
}
