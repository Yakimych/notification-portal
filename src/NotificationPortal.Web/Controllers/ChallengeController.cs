using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotificationPortal.Web.Core;
using NotificationPortal.Web.Data;
using NotificationPortal.Web.Hubs;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.Controllers
{
    public class ChallengeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ChallengeService _challengeService;
        private readonly IHubContext<ChallengeHub> _challengeHubContext;

        public ChallengeController(
            ApplicationDbContext dbContext,
            ChallengeService challengeService,
            IHubContext<ChallengeHub> challengeHubContext)
        {
            _dbContext = dbContext;
            _challengeService = challengeService;
            _challengeHubContext = challengeHubContext;
        }

        [HttpGet]
        public IActionResult SendChallenge()
        {
            return View(new SendChallengeModel());
        }

        [HttpGet]
        public async Task<IActionResult> ChallengeList()
        {
            var notifications = await _dbContext.Notifications.ToListAsync();
            return View(new ChallengeListViewModel { Challenges = notifications });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendChallenge(SendChallengeModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var newNotification = new Notification
            {
                CommunityName = model.CommunityName,
                FromPlayer = model.FromPlayer,
                ToPlayer = model.ToPlayer,
                Status = ChallengeStatus.Challenged,
                Date = DateTime.UtcNow
            };

            try
            {
                await _dbContext.Notifications.AddAsync(newNotification);
                await _dbContext.SaveChangesAsync();

                // TODO: Can we pass a slimmed down version of the notification object instead?
                await _challengeHubContext.Clients.All.SendAsync(
                    "NewChallengeIssued",
                    newNotification.Id,
                    newNotification.CommunityName,
                    newNotification.FromPlayer,
                    newNotification.ToPlayer,
                    ChallengeStatus.Challenging.ToString(),
                    newNotification.Date.FormatDateTime());

                BackgroundJob.Enqueue(() =>
                    _challengeService.InitiateChallenge(newNotification.Id, model.CommunityName, model.FromPlayer,
                        model.ToPlayer));

                model.RequestStatusMessage = "Challenge queued for sending";
                return View(model);
            }
            catch (Exception ex)
            {
                model.RequestStatusMessage = $"Error: {ex.Message}"; // TODO: Log exception instead
                return View(model);
            }
        }
    }
}
