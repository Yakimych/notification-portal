using System;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace NotificationPortal.Web.Data
{

    // public class ApplicationUser : IdentityUser
    // {
    // }

    public class ApplicationDbContext : ApiAuthorizationDbContext<IdentityUser> //IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
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
        public int Id { get; set; }

        public string CommunityName { get; set; }

        public string FromPlayer { get; set; }

        public string ToPlayer { get; set; }

        public ChallengeType Type { get; set; }

        public DateTime Date { get; set; }
    }
}
