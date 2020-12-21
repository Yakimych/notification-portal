using System.Collections.Generic;
using NotificationPortal.Data;

namespace NotificationPortal.Web.Models
{
    public record NotificationsViewModel(List<ChallengeNotification> Notifications);
}
