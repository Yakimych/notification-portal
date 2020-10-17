using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using NotificationPortal.Web.Core;
using NotificationPortal.Web.Data;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.Controllers
{
    public class ChallengeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ChallengeService _challengeService;

        public ChallengeController(ApplicationDbContext dbContext, ChallengeService challengeService)
        {
            _dbContext = dbContext;
            _challengeService = challengeService;
        }

        [HttpGet]
        public IActionResult SendChallenge()
        {
            return View(new SendChallengeModel());
        }

        [HttpGet]
        public IActionResult ChallengeList()
        {
            return View(new ChallengeListViewModel());
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
                Type = ChallengeType.Challenged,
                Date = DateTime.UtcNow
            };

            try
            {
                await _dbContext.Notifications.AddAsync(newNotification);
                await _dbContext.SaveChangesAsync();

                BackgroundJob.Enqueue(() =>
                    _challengeService.SendMessage(model.CommunityName, model.FromPlayer, model.ToPlayer));

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
