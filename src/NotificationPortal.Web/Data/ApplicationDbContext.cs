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

        public DbSet<Notification> Notifications { get; set; }
    }

    public enum ChallengeType
    {
        Challenged,
        Accepted,
        Declined
    }

    public class Notification
    {
        public int Id { get; init; }

        public string CommunityName { get; init; }

        public string FromPlayer { get; init; }

        public string ToPlayer { get; init; }

        public ChallengeType Type { get; init; }

        public DateTime Date { get; init; }
    }
}
