using System.Collections.Generic;
using NotificationPortal.Web.Data;

namespace NotificationPortal.Web.Models
{
    public record ChallengeListViewModel
    {
        public List<ChallengeEntry> Challenges { get; init; }
    }
}
