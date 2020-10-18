using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NotificationPortal.Web.Core;
using NotificationPortal.Web.Data;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.Controllers
{
    public class ChallengeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ChallengeService _challengeService;
        private readonly ILogger<HomeController> _logger;

        public ChallengeController(
            ApplicationDbContext dbContext, ChallengeService challengeService, ILogger<HomeController> logger)
        {
            _dbContext = dbContext;
            _challengeService = challengeService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult SendChallenge()
        {
            return View(new SendChallengeModel());
        }

        [HttpGet]
        public IActionResult ChallengeList()
        {
            _logger.Log(LogLevel.Information, "ChallengeList action called");
            return View(new ChallengeListViewModel
            {
                Challenges = new List<Notification>
                {
                    new Notification
                    {
                        Id = 1,
                        CommunityName = "test",
                        FromPlayer = "testPlayer1",
                        ToPlayer = "testPlayer2",
                        Date = DateTime.Now,
                        Type = ChallengeType.Challenged
                    },
                    new Notification
                    {
                        Id = 2,
                        CommunityName = "test",
                        FromPlayer = "testPlayer3",
                        ToPlayer = "testPlayer4",
                        Date = DateTime.Now,
                        Type = ChallengeType.Accepted
                    },
                    new Notification
                    {
                        Id = 3,
                        CommunityName = "test",
                        FromPlayer = "testPlayer5",
                        ToPlayer = "testPlayer6",
                        Date = DateTime.Now,
                        Type = ChallengeType.Declined
                    }
                }
            });
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
