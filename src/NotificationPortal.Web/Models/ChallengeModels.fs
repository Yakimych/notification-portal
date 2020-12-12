module NotificationPortal.Web.Models.ChallengeModels

open System
open System.ComponentModel.DataAnnotations
open NotificationPortal.Data
// open Swashbuckle.AspNetCore.Annotations

// TODO: Use SwaggerSchema instead after this issue is resolved: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1920
// [SwaggerSchema(Required = new[] { nameof(Id), nameof(ToPlayer) })]
type ChallengeModel = {
    [<Required>]
    Id: int

    [<Required>]
    CommunityName: string

    [<Required>]
    FromPlayer: string

    [<Required>]
    ToPlayer: string

    [<Required>]
    Status: ChallengeStatus

    [<Required>]
    Date: DateTime
}

type ChallengeCollectionModel = { [<Required>] Challenges: ChallengeModel list }
