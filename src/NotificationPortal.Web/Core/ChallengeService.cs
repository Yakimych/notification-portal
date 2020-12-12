using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationPortal.Data;
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
        public async Task<ChallengeEntry> CreateChallengeSendNotificationAndSaveEverythingToTheDatabase(
            SendChallengeModel model)
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
            var challengeToSave = challenge with { Date = DateTime.UtcNow, Status = ChallengeStatus.Challenged };
            _dbContext.ChallengeEntries.Update(challengeToSave);

            // Save new Notification
            var challengeNotificationToSave = challengeNotification with { ChallengeEntryId = challengeToSave.Id };
            await _dbContext.Notifications.AddAsync(challengeNotificationToSave);
            await _dbContext.SaveChangesAsync();

            await _challengeHubContext.Clients.All.SendAsync("ChallengeStatusChanged", challengeToSave.Id,
                challengeToSave.Status.ToString());
        }

        public async Task RespondToChallenge(ChallengeEntry challenge, NotificationType response)
        {
            // TODO: Is it possible to handle errors here? Test with spaces in player names and try/catch
            var challengeNotification =
                await _firebaseMessagingService.SendMessageResponseToChallenge(challenge, response);

            // Update Challenge status and timestamp
            var challengeToSave =
                challenge with
                {
                    Date = DateTime.UtcNow,
                    Status =
                        response == NotificationType.Accepted
                            ? ChallengeStatus.Accepted
                            : ChallengeStatus.Declined
                };
            _dbContext.ChallengeEntries.Update(challengeToSave);

            // Save new Notification
            var challengeNotificationToSave = challengeNotification with { ChallengeEntryId = challengeToSave.Id };
            await _dbContext.Notifications.AddAsync(challengeNotificationToSave);
            await _dbContext.SaveChangesAsync();

            await _challengeHubContext.Clients.All.SendAsync("ChallengeStatusChanged", challengeToSave.Id,
                challengeToSave.Status.ToString());
        }

        // TODO: SetStatus?
        public async Task<OperationResult> AcceptChallenge(int challengeId)
        {
            var challengeToAccept = await _dbContext.ChallengeEntries.FindAsync(challengeId);
            if (challengeToAccept == null)
                return OperationResult.NotFound;

            var acceptedChallenge = challengeToAccept with { Status = ChallengeStatus.Accepting };
            _dbContext.Entry(challengeToAccept).State = EntityState.Detached;
            _dbContext.Update(acceptedChallenge);

            await _dbContext.SaveChangesAsync();

            // TODO: Extract into service/function
            await _challengeHubContext.Clients.All.SendAsync(
                "ChallengeStatusChanged",
                acceptedChallenge.Id,
                acceptedChallenge.Status.ToString());

            BackgroundJob.Enqueue(() => RespondToChallenge(acceptedChallenge, NotificationType.Accepted));

            return OperationResult.Ok;
        }

        public async Task<OperationResult> DeclineChallenge(int challengeId)
        {
            var challengeToDecline = await _dbContext.ChallengeEntries.FindAsync(challengeId);
            if (challengeToDecline == null)
                return OperationResult.NotFound;

            var declinedChallenge = challengeToDecline with { Status = ChallengeStatus.Declining };
            _dbContext.Entry(challengeToDecline).State = EntityState.Detached;
            _dbContext.Update(declinedChallenge);
            await _dbContext.SaveChangesAsync();

            // TODO: Extract into service/function
            await _challengeHubContext.Clients.All.SendAsync(
                "ChallengeStatusChanged",
                declinedChallenge.Id,
                declinedChallenge.Status.ToString());

            BackgroundJob.Enqueue(() => RespondToChallenge(declinedChallenge, NotificationType.Declined));

            return OperationResult.Ok;
        }
    }

    public enum OperationResult
    {
        NotFound,
        Ok
    }
}
