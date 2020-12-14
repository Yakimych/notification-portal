namespace NotificationPortal.Web

open System.Threading.Tasks
open Microsoft.AspNetCore.SignalR
open NotificationPortal.Data

type ChallengeHub() =
    inherit Hub()

    member this.AcceptChallenge (challengeId: int) =
        ()

    member this.DeclineChallenge (challengeId: int) =
        ()
