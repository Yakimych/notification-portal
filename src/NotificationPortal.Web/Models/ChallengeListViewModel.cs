using System;
using System.Collections.Generic;

namespace NotificationPortal.Web.Models
{
    public class ChallengeModel
    {
        public int Id { get; init; }

        public string CommunityName { get; init; }

        public string FromPlayer { get; init; }

        public string ToPlayer { get; init; }

        public string Type { get; init; }

        public DateTime Date { get; init; }
    }

    public class ChallengeCollectionModel
    {
        public List<ChallengeModel> Challenges { get; init; }
    }
}
