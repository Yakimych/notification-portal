using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NotificationPortal.Web.Data;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(ILogger<NotificationsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var notification1 = new Notification
            {
                CommunityName = "test",
                FromPlayer = "player1",
                ToPlayer = "player2",
                Type = ChallengeType.Challenged,
                Date = DateTime.UtcNow
            };

            var notification2 = new Notification
            {
                CommunityName = "test",
                FromPlayer = "player2",
                ToPlayer = "player1",
                Type = ChallengeType.Accepted,
                Date = DateTime.UtcNow
            };

            var notification3 = new Notification
            {
                CommunityName = "test2",
                FromPlayer = "Player3",
                ToPlayer = "Player4",
                Type = ChallengeType.Challenged,
                Date = DateTime.UtcNow
            };

            var notificationsViewModel = new NotificationsViewModel
            {
                Notifications = new List<Notification> { notification1, notification2, notification3 }
            };

            return View(notificationsViewModel);
        }
    }
}
