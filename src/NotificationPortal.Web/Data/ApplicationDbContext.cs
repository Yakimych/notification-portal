﻿using System;
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
        public int Id { get; set; }

        public string CommunityName { get; set; }

        public string FromPlayer { get; set; }

        public string ToPlayer { get; set; }

        public ChallengeType Type { get; set; }

        public DateTime Date { get; set; }
    }
}
