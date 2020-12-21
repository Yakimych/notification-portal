using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationPortal.Data
{
    public static class DbInitializer
    {
        private const string AdminRole = "Admin";

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            if (dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                await dbContext.Database.MigrateAsync();
            }

            var config = serviceProvider.GetRequiredService<IConfiguration>();

            var adminEmail = config["admin_username"];
            var password = config["admin_password"];

            var adminUserId = await EnsureUser(serviceProvider, adminEmail, password);
            await EnsureRole(serviceProvider, adminUserId, AdminRole);

            await dbContext.SaveChangesAsync();
        }

        private static T TryResolve<T>(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetService<T>();
            if (userManager is null)
                throw new Exception($"{typeof(T)} has not been registered.");

            return userManager;
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string email, string password)
        {
            var userManager = TryResolve<UserManager<IdentityUser>>(serviceProvider);
            var user = await userManager.FindByNameAsync(email);
            if (user is null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, password);
            }

            if (user is null)
                throw new Exception("The password is probably not strong enough!");

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            var roleManager = TryResolve<RoleManager<IdentityRole>>(serviceProvider);

            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

            var userManager = TryResolve<UserManager<IdentityUser>>(serviceProvider);

            var user = await userManager.FindByIdAsync(uid);
            if (user == null)
                throw new Exception("The password was probably not strong enough!");

            return await userManager.AddToRoleAsync(user, role);
        }
    }
}
