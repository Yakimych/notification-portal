namespace NotificationPortal.Web

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy;
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting


open Hangfire
open Hangfire.Dashboard
open Hangfire.Storage.SQLite
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Identity
open Microsoft.AspNetCore.Hosting
open Microsoft.EntityFrameworkCore
open NotificationPortal.Data
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
//open NotificationPortal.Web.Core
//open NotificationPortal.Web.Hubs
open Microsoft.OpenApi.Models
open System.Text.Json.Serialization


type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        services.AddDbContext<ApplicationDbContext>(fun options ->
            options.UseSqlite(this.Configuration.GetConnectionString("DefaultConnection")) |> ignore
        ) |> ignore

        services.AddDefaultIdentity<IdentityUser>(fun options -> options.SignIn.RequireConfirmedAccount <- true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
        |> ignore

        services
            .AddControllersWithViews()
            .AddJsonOptions(fun options -> options.JsonSerializerOptions.Converters.Add(JsonStringEnumConverter()))
            .AddRazorRuntimeCompilation() |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =

        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        else
            app.UseExceptionHandler("/Home/Error") |> ignore
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts() |> ignore

        app.UseHttpsRedirection() |> ignore
        app.UseStaticFiles() |> ignore

        app.UseRouting() |> ignore

        app.UseAuthentication() |> ignore
        app.UseAuthorization() |> ignore

        app.UseEndpoints(fun endpoints ->
            endpoints.MapControllerRoute(
                name = "default",
                pattern = "{controller=Home}/{action=Index}/{id?}") |> ignore
            endpoints.MapRazorPages() |> ignore) |> ignore

    member val Configuration : IConfiguration = null with get, set
