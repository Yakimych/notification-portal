using System.Collections.Generic;
using NotificationPortal.Web.Data;

namespace NotificationPortal.Web.Models
{
    public record NotificationsViewModel
    {
        public List<ChallengeNotification> Notifications { get; init; }
    }
}
