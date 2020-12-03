﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationPortal.Web.Core;
using NotificationPortal.Web.Data;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.Controllers
{
    public class ChallengeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ChallengeService _challengeService;

        public ChallengeController(
            ApplicationDbContext dbContext,
            ChallengeService challengeService)
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
        public async Task<IActionResult> ChallengeList()
        {
            var challengeEntries = await _dbContext.ChallengeEntries.ToListAsync();
            return View(new ChallengeListViewModel { Challenges = challengeEntries });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendChallenge(SendChallengeModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _challengeService.CreateChallengeSendNotificationAndSaveEverythingToTheDatabase(model);
                return View(model with { RequestStatusMessage = "Challenge queued for sending" });
            }
            catch (Exception ex)
            {
                return View(model with { RequestStatusMessage = $"Error: {ex.Message}" }); // TODO: Log exception instead
            }
        }
    }
}
