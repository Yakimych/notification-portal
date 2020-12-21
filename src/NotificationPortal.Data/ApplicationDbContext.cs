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

        public DbSet<ChallengeNotification> Notifications => Set<ChallengeNotification>();
        public DbSet<ChallengeEntry> ChallengeEntries => Set<ChallengeEntry>();
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

    public record ChallengeEntry(string CommunityName, string FromPlayer, string ToPlayer, ChallengeStatus Status, DateTime Date)
    {
        public int Id { get; private init; }
    }

    public record ChallengeNotification(string Topic, string Message, string FromPlayer, string FirebaseResponse, NotificationType Type, DateTime Date)
    {
        public int Id { get; private init; }
        public int ChallengeEntryId { get; init; }

        private ChallengeEntry? _challenge;
        public ChallengeEntry Challenge
        {
            get => _challenge ?? throw new InvalidOperationException($"Uninitialized property: {nameof(Challenge)}");
            set => _challenge = value;
        }
    }
}
