open System
open Farmer
open Farmer.Builders

let commandLineArgs = Environment.GetCommandLineArgs()

match commandLineArgs |> List.ofArray with
| [] | [_] -> Console.WriteLine("No command line argument specified.")
| _applicationName :: publishedAppFolder :: _ ->

    let resourceGroupName = "NotificationPortalGroup"
    let webApplicationName = "RelogifyNotificationPortal"

    let myWebApp = webApp {
        name webApplicationName
        zip_deploy publishedAppFolder
        app_insights_off
    }

    let deployment = arm {
        location Location.NorthEurope
        add_resource myWebApp
    }

    deployment
    |> Deploy.execute resourceGroupName Deploy.NoParameters
    |> ignore
