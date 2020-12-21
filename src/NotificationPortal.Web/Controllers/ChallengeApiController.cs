using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationPortal.Data;
using NotificationPortal.Web.ActorModel;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.Controllers
{
    [Route("api/challenges")]
    [ApiController]
    public class ChallengeApiController : ControllerBase
    {
        private readonly RelogifyActorModel _relogifyActorModel;

        // TODO: Do not use dbContext directly in controllers
        private readonly ApplicationDbContext _dbContext;

        public ChallengeApiController(ApplicationDbContext dbContext, RelogifyActorModel relogifyActorModel)
        {
            _dbContext = dbContext;
            _relogifyActorModel = relogifyActorModel;
        }

        [HttpGet]
        public async Task<ActionResult<ChallengeCollectionModel>> GetChallenges()
        {
            var challenges =
                await _dbContext.ChallengeEntries.Select(c => c.ToChallengeModel()).ToListAsync();

            return Ok(new ChallengeCollectionModel(Challenges: challenges));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChallenge(int id)
        {
            var challenge = await _dbContext.ChallengeEntries.FindAsync(id);
            if (challenge == null)
                return NotFound();

            return Ok(challenge);
        }

        [HttpPost]
        public IActionResult CreateChallenge(SendChallengeModel model)
        {
            try
            {
                _relogifyActorModel.PublishMessage(new ChallengeIssuedMessage(SendChallengeModel: model));
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
            _relogifyActorModel.PublishMessage(new ChallengeAcceptedMessage(ChallengeEntryId: id));
            return NoContent();
        }

        [HttpPost("rpc/decline/{id}")]
        public IActionResult DeclineChallenge(int id)
        {
            _relogifyActorModel.PublishMessage(new ChallengeDeclinedMessage(ChallengeEntryId: id));
            return NoContent();
        }
    }
}
