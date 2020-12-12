namespace NotificationPortal.Web

open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open NotificationPortal.Data
open NotificationPortal.Web.Controllers

module Program =
    let exitCode = 0

    let CreateHostBuilder args =
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(fun webBuilder ->
                webBuilder.UseStartup<Startup>() |> ignore
            )

    [<EntryPoint>]
    let main args =
        let host = CreateHostBuilder(args).Build()

        use scope = host.Services.CreateScope()
        let services = scope.ServiceProvider

        try
            DbInitializer.Initialize(services).Wait()
        with
        | ex ->
            let logger = services.GetRequiredService<ILogger<HomeController>>() // TODO: ILogger<Program>
            logger.LogError(ex, "Failed creating or migrating the Database");

        host.Run()

        exitCode
