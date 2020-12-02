using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using NotificationPortal.Web.Data;
using NotificationPortal.Web.Hubs;
using NotificationPortal.Web.Models;

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

        // TODO: Do not pass the SendChallengeModel down to services
        public async Task<ChallengeEntry> CreateChallengeSendNotificationAndSaveEverythingToTheDatabase(SendChallengeModel model)
        {
            var newChallenge = new ChallengeEntry
            {
                CommunityName = model.CommunityName,
                FromPlayer = model.FromPlayer,
                ToPlayer = model.ToPlayer,
                Status = ChallengeStatus.Challenging,
                Date = DateTime.UtcNow
            };

            await _dbContext.ChallengeEntries.AddAsync(newChallenge);
            await _dbContext.SaveChangesAsync();

            // TODO: Can we pass a slimmed down version of the notification object instead?
            await _challengeHubContext.Clients.All.SendAsync(
                "NewChallengeIssued",
                newChallenge.Id,
                newChallenge.CommunityName,
                newChallenge.FromPlayer,
                newChallenge.ToPlayer,
                newChallenge.Status.ToString(),
                newChallenge.Date.FormatDateTime());

            BackgroundJob.Enqueue(() => InitiateChallenge(newChallenge));

            return newChallenge;
        }

        public async Task InitiateChallenge(ChallengeEntry challenge)
        {
            // TODO: Is it possible to handle errors here? Test with spaces in player names and try/catch
            var challengeNotification =
                await _firebaseMessagingService.SendMessage(challenge);

            // Update Challenge status and timestamp
            _dbContext.ChallengeEntries.Attach(challenge);
            challenge.Date = DateTime.UtcNow;
            challenge.Status = ChallengeStatus.Challenged;

            // Save new Notification
            challengeNotification.ChallengeEntryId = challenge.Id;
            await _dbContext.Notifications.AddAsync(challengeNotification);
            await _dbContext.SaveChangesAsync();

            await _challengeHubContext.Clients.All.SendAsync("ChallengeStatusChanged", challenge.Id,
                challenge.Status.ToString());
        }

        public async Task RespondToChallenge(ChallengeEntry challenge, NotificationType response)
        {
            // TODO: Is it possible to handle errors here? Test with spaces in player names and try/catch
            var challengeNotification =
                await _firebaseMessagingService.SendMessageResponseToChallenge(challenge, response);

            // Update Challenge status and timestamp
            _dbContext.ChallengeEntries.Attach(challenge);
            challenge.Date = DateTime.UtcNow;
            challenge.Status = response == NotificationType.Accepted
                ? ChallengeStatus.Accepted
                : ChallengeStatus.Declined;

            // Save new Notification
            challengeNotification.ChallengeEntryId = challenge.Id;
            await _dbContext.Notifications.AddAsync(challengeNotification);
            await _dbContext.SaveChangesAsync();

            await _challengeHubContext.Clients.All.SendAsync("ChallengeStatusChanged", challenge.Id,
                challenge.Status.ToString());
        }

        public async Task<OperationResult> AcceptChallenge(int challengeId)
        {
            var challengeToAccept = await _dbContext.ChallengeEntries.FindAsync(challengeId);
            if (challengeToAccept == null)
                return OperationResult.NotFound;

            challengeToAccept.Status = ChallengeStatus.Accepting;
            await _dbContext.SaveChangesAsync();

            // TODO: Extract into service/function
            await _challengeHubContext.Clients.All.SendAsync(
                "ChallengeStatusChanged",
                challengeToAccept.Id,
                challengeToAccept.Status.ToString());

            BackgroundJob.Enqueue(() => RespondToChallenge(challengeToAccept, NotificationType.Accepted));

            return OperationResult.Ok;
        }

        public async Task<OperationResult> DeclineChallenge(int challengeId)
        {
            var challengeToDecline = await _dbContext.ChallengeEntries.FindAsync(challengeId);
            if (challengeToDecline == null)
                return OperationResult.NotFound;

            challengeToDecline.Status = ChallengeStatus.Declining;
            await _dbContext.SaveChangesAsync();

            // TODO: Extract into service/function
            await _challengeHubContext.Clients.All.SendAsync(
                "ChallengeStatusChanged",
                challengeToDecline.Id,
                challengeToDecline.Status.ToString());

            BackgroundJob.Enqueue(() => RespondToChallenge(challengeToDecline, NotificationType.Declined));

            return OperationResult.Ok;
        }
    }

    public enum OperationResult
    {
        NotFound,
        Ok
    }
}
