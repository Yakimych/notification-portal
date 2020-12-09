using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationPortal.Web.Core;
using NotificationPortal.Web.Data;
using NotificationPortal.Web.Models;

namespace NotificationPortal.Web.Controllers
{
    [Route("api/challenges")]
    [ApiController]
    public class ChallengeApiController : ControllerBase
    {
        private readonly ChallengeService _challengeService;
        // TOOD: Do not use dbContext directly in controllers
        private readonly ApplicationDbContext _dbContext;

        public ChallengeApiController(ChallengeService challengeService, ApplicationDbContext dbContext)
        {
            _challengeService = challengeService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetChallenges()
        {
            var challenges =
                await _dbContext.ChallengeEntries.Select(c => c.ToChallengeModel()).ToListAsync();

            return Ok(new ChallengeCollectionModel { Challenges = challenges });
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
        public async Task<IActionResult> CreateChallenge(SendChallengeModel model)
        {
            try
            {
                var createdChallenge =
                    await _challengeService.CreateChallengeSendNotificationAndSaveEverythingToTheDatabase(model);
                return Ok(createdChallenge);
            }
            catch (Exception ex)
            {
                // TODO: Do not expose raw exception information
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("rpc/accept/{id}")]
        public async Task<IActionResult> AcceptChallenge(int id)
        {
            var result = await _challengeService.AcceptChallenge(id);
            if (result == OperationResult.NotFound)
                return NotFound();

            return NoContent();
        }

        [HttpPost("rpc/decline/{id}")]
        public async Task<IActionResult> DeclineChallenge(int id)
        {
            var result = await _challengeService.DeclineChallenge(id);
            if (result == OperationResult.NotFound)
                return NotFound();

            return NoContent();
        }
    }
}
