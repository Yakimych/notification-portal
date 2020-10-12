using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationPortal.Web.Data;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public NotificationsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var notifications = await _dbContext.Notifications.ToListAsync();

            var notificationsViewModel = new NotificationsViewModel
            {
                Notifications = notifications
            };

            return View(notificationsViewModel);
        }
    }
}
