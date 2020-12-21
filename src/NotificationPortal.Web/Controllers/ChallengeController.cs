using Microsoft.AspNetCore.Mvc;
using NotificationPortal.Web.ActorModel;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.Controllers
{
    public class ChallengeController : Controller
    {
        private readonly RelogifyActorModel _actorModel;

        public ChallengeController(RelogifyActorModel actorModel)
        {
            _actorModel = actorModel;
        }

        [HttpGet]
        public IActionResult SendChallenge()
        {
            return View(
                new SendChallengeModel(CommunityName: "", FromPlayer: "", ToPlayer: "", RequestStatusMessage: null));
        }

        [HttpGet]
        public IActionResult ChallengeList()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendChallenge(SendChallengeModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _actorModel.PublishMessage(new ChallengeIssuedMessage(SendChallengeModel: model));

            return View(model with { RequestStatusMessage = "Challenge queued for sending" });
        }
    }
}
