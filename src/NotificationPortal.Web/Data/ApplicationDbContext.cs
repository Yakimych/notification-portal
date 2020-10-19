using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NotificationPortal.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ChallengeNotification> Notifications { get; set; }
        public DbSet<ChallengeEntry> ChallengeEntries { get; set; }
    }

    public enum NotificationType
    {
        Challenged,
        Accepted,
        Declined
    }

    public enum ChallengeStatus
    {
        Challenging,
        Challenged,
        Accepting,
        Accepted,
        Declining,
        Declined
    }

    public class ChallengeEntry
    {
        public int Id { get; set; }

        public string CommunityName { get; set; }

        public string FromPlayer { get; set; }

        public string ToPlayer { get; set; }

        public ChallengeStatus Status { get; set; }

        public DateTime Date { get; set; }
    }

    public class ChallengeNotification
    {
        public int Id { get; set; }

        public int ChallengeEntryId { get; set; }

        public ChallengeEntry Challenge { get; set; }

        public string Topic { get; set; }

        public string Message { get; set; }

        public string FromPlayer { get; set; }

        public string FirebaseResponse { get; set; }

        public NotificationType Type { get; set; }

        public DateTime Date { get; set; }
    }
}
