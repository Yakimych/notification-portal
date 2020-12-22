using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using NotificationPortal.Web.ActorModel;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly RelogifyActorModel _relogifyActorModel;

        public NotificationsController(RelogifyActorModel relogifyActorModel)
        {
            _relogifyActorModel = relogifyActorModel;
        }

        public async Task<IActionResult> Index()
        {
            var notificationsResponse =
                await _relogifyActorModel.NotificationActor
                    .Ask<GetNotificationsResponse>(new GetNotificationsMessage());

            var notificationsViewModel = new NotificationsViewModel
            {
                Notifications = notificationsResponse.Notifications
            };

            return View(notificationsViewModel);
        }
    }
}
