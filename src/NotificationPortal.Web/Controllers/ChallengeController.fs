namespace NotificationPortal.Web.Controllers

open Microsoft.AspNetCore.Mvc
open NotificationPortal.Data
open System.ComponentModel.DataAnnotations

[<CLIMutable>]
type SendChallengeModel = {
    [<Required>]
    CommunityName: string

    [<Required>]
    FromPlayer: string

    [<Required>]
    ToPlayer: string

    RequestStatusMessage: string
}

type ChallengeController () =
    inherit Controller()

    [<HttpGet>]
    member this.SendChallenge () =
        let emptySendChallengeModel: SendChallengeModel =
            { CommunityName = ""
              FromPlayer = ""
              ToPlayer = ""
              RequestStatusMessage = "" }

        this.View(emptySendChallengeModel)

    [<HttpGet>]
    member this.ChallengeList () =
        this.View()

    [<HttpPost>]
    [<ValidateAntiForgeryToken>]
    member this.SendChallenge (model: SendChallengeModel) =
        if not this.ModelState.IsValid then
            this.View(model)
        else
            // TODO: Call challengeService, try/catch, return success or error
            this.View({ model with RequestStatusMessage = "Challenge queued for sending" })


