using System;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using NotificationPortal.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationPortal.Web.Hubs;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using NotificationPortal.Web.ActorModel;

namespace NotificationPortal.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services
                .AddControllersWithViews()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "NotificationPortal", Version = "v1" }));
            services.AddSignalR();

            services.AddHangfire(configuration =>
                configuration
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSQLiteStorage()
            );
            services.AddHangfireServer();

            services.AddSingleton(provider =>
            {
                var configuration = provider.GetService<IConfiguration>();
                const string FirebaseJsonConfigurationKey = "firebase_json";
                var firebaseConfigurationJsonString = configuration?[FirebaseJsonConfigurationKey];

                if (firebaseConfigurationJsonString is null)
                    throw new Exception($"Firebase configuration key: '{FirebaseJsonConfigurationKey}' is missing");

                return new FirebaseMessagingService(firebaseConfigurationJsonString);
            });

            services.AddSingleton(provider =>
            {
                var serviceScopeFactory = provider.GetService<IServiceScopeFactory>();
                var relogifyActorModel = RelogifyActorSystem.CreateModel(serviceScopeFactory);

                return relogifyActorModel;
            });

            services.AddScoped<NotificationPersistence>();
            services.AddScoped<ChallengePersistence>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotificationPortal Api v1"));

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseDefaultFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/hangfire",
                new DashboardOptions { Authorization = new[] { new HangfireAuthorizationFilter() } });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

                endpoints.MapHub<ChallengeHub>("/challengehub");
            });
        }

        public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                var httpContext = context.GetHttpContext();

                // Allow all authenticated users to see the Dashboard
                // TODO: Restrict to admin
                return httpContext.User.Identity.IsAuthenticated;
            }
        }
    }
}
