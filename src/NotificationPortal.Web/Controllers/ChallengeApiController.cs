using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using NotificationPortal.Web.ActorModel;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.Controllers
{
    [Route("api/challenges")]
    [ApiController]
    public class ChallengeApiController : ControllerBase
    {
        private readonly RelogifyActorModel _relogifyActorModel;

        public ChallengeApiController(RelogifyActorModel relogifyActorModel)
        {
            _relogifyActorModel = relogifyActorModel;
        }

        [HttpGet]
        public async Task<ActionResult<ChallengeCollectionModel>> GetChallenges()
        {
            var challengesResponse =
                await _relogifyActorModel.ChallengeActor.Ask<GetChallengesResponse>(new GetChallengesMessage());

            return Ok(new ChallengeCollectionModel
                { Challenges = challengesResponse.ChallengeEntries.Select(c => c.ToChallengeModel()).ToList() });
        }

        [HttpPost]
        public IActionResult CreateChallenge(SendChallengeModel model)
        {
            try
            {
                _relogifyActorModel.PublishMessage(
                    new ChallengeIssuedMessage(SendChallengeModel: model, TimeStamp: DateTime.UtcNow));
                return Ok();
            }
            catch (Exception ex)
            {
                // TODO: Do not expose raw exception information
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("rpc/accept/{id}")]
        public IActionResult AcceptChallenge(int id)
        {
            _relogifyActorModel.PublishMessage(
                new ChallengeAcceptedMessage(ChallengeEntryId: id, TimeStamp: DateTime.UtcNow));
            return NoContent();
        }

        [HttpPost("rpc/decline/{id}")]
        public IActionResult DeclineChallenge(int id)
        {
            _relogifyActorModel.PublishMessage(
                new ChallengeDeclinedMessage(ChallengeEntryId: id, TimeStamp: DateTime.UtcNow));
            return NoContent();
        }
    }
}
