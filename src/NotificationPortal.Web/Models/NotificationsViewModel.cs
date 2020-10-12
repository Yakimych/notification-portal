using System;
using System.Collections.Generic;

namespace NotificationPortal.Web.Models
{
    public enum ChallengeType
    {
        Challenged,
        Accepted,
        Declined
    }

    public class NotificationsViewModel
    {
        public List<NotificationModel> Notifications { get; init; }
    }

    public class NotificationModel
    {
        public string CommunityName { get; init; }

        public string FromPlayer { get; init; }

        public string ToPlayer { get; init; }

        public ChallengeType Type { get; init; }

        public DateTime Date { get; init; }
    }
}
