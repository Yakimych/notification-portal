namespace NotificationPortal.Web.Controllers

open System.Collections.Generic
open Microsoft.AspNetCore.Mvc
open NotificationPortal.Data
open Microsoft.EntityFrameworkCore

type NotificationsViewModel = { Notifications: List<ChallengeNotification> }

type NotificationsController (dbContext : ApplicationDbContext) =
    inherit Controller()

    member this.Index () =
        async {
            let! notifications = dbContext.Notifications.ToListAsync() |> Async.AwaitTask
            let notificationsViewModel: NotificationsViewModel = { Notifications = notifications }

            return this.View(notificationsViewModel)
        }
