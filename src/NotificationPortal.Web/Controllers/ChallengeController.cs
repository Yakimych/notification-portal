using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotificationPortal.Web.Data;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.Controllers
{
    public class ChallengeController : Controller
    {
        public const string StatusMessageKey = "STATUS_MESSAGE";

        private readonly ApplicationDbContext _dbContext;

        public ChallengeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult SendChallenge()
        {
            ViewData[StatusMessageKey] = TempData[StatusMessageKey];
            return View(new SendChallengeModel());
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

                TempData[StatusMessageKey] = "Challenge queued for sending";
                return RedirectToAction("SendChallenge");
            }
            catch (Exception ex)
            {
                ViewData[StatusMessageKey] = $"Error: {ex.Message}"; // TODO: Log exception instead
                return View(model);
            }
        }
    }
}
