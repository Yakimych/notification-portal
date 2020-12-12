namespace NotificationPortal.Web.Controllers

open System
open Microsoft.AspNetCore.Mvc
open NotificationPortal.Data
open NotificationPortal.Web.Models.ChallengeModels
open System.Threading.Tasks

[<Route("api/challenges")>]
[<ApiController>]
type ChallengeApiController (dbContext : ApplicationDbContext) =
    inherit ControllerBase()

    [<HttpGet>]
    member this.GetChallenges () : Task<ChallengeCollectionModel> =
        let fakeChallengeModel: ChallengeModel =
            { Id = 1
              CommunityName = "TestCommunity"
              FromPlayer = "TestCommunity"
              ToPlayer = "TestCommunity"
              Status = ChallengeStatus.Challenged
              Date = DateTime.UtcNow }

        let fakeChallengeCollection: ChallengeCollectionModel = { Challenges = [ fakeChallengeModel ] }
        Task.Run(fun () -> fakeChallengeCollection)
