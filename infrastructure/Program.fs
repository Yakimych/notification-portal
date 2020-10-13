open System
open Farmer
open Farmer.Builders

let commandLineArgs = Environment.GetCommandLineArgs()

match commandLineArgs |> List.ofArray with
| [] | [_] -> Console.WriteLine("No command line argument specified.")
| _applicationName :: publishedAppFolder :: adminUserName :: adminPassword :: _ ->

    let resourceGroupName = "NotificationPortalGroup"
    let webApplicationName = "RelogifyNotificationPortal"

    let myWebApp = webApp {
        name webApplicationName
        zip_deploy publishedAppFolder
        settings [ ("admin_username", adminUserName); ("admin_password", adminPassword) ]
        app_insights_off
    }

    let deployment = arm {
        location Location.NorthEurope
        add_resource myWebApp
    }

    deployment
    |> Deploy.execute resourceGroupName Deploy.NoParameters
    |> ignore
| _ -> Console.WriteLine("There should be exactly 3 command line argument specified: publishedAppFolder, adminUserName and adminPassword.")
