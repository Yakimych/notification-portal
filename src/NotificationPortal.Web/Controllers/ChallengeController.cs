using Microsoft.AspNetCore.Mvc;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.Controllers
{
    public class ChallengeController : Controller
    {
        public const string StatusMessageKey = "STATUS_MESSAGE";

        [HttpGet]
        public IActionResult SendChallenge()
        {
            ViewData[StatusMessageKey] = TempData[StatusMessageKey];
            return View(new SendChallengeModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendChallenge(SendChallengeModel sendChallengeModel)
        {
            if (!ModelState.IsValid)
                return View(sendChallengeModel);

            TempData[StatusMessageKey] = "Challenge queued for sending";
            return RedirectToAction("SendChallenge");
        }
    }
}
