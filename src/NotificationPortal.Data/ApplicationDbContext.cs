using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NotificationPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ChallengeNotification> Notifications { get; init; }
        public DbSet<ChallengeEntry> ChallengeEntries { get; init; }
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

    public record ChallengeEntry
    {
        public int Id { get; init; }

        public string CommunityName { get; init; }

        public string FromPlayer { get; init; }

        public string ToPlayer { get; init; }

        public ChallengeStatus Status { get; init; }

        public DateTime Date { get; init; }
    }

    public record ChallengeNotification
    {
        public int Id { get; init; }

        public int ChallengeEntryId { get; init; }

        public ChallengeEntry Challenge { get; init; }

        public string Topic { get; init; }

        public string Message { get; init; }

        public string FromPlayer { get; init; }

        public string FirebaseResponse { get; init; }

        public NotificationType Type { get; init; }

        public DateTime Date { get; init; }
    }
}
