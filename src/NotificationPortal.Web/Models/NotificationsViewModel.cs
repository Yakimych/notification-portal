using System.Collections.Immutable;
using NotificationPortal.Data;

namespace NotificationPortal.Web.Models
{
    public record NotificationsViewModel
    {
        public ImmutableList<ChallengeNotification> Notifications { get; init; }
    }
}
