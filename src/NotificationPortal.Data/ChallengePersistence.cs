using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NotificationPortal.Data
{
    public class ChallengePersistence
    {
        private readonly ApplicationDbContext _dbContext;

        public ChallengePersistence(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ImmutableList<ChallengeEntry>> GetAllFromDb() => _dbContext.ChallengeEntries.ToImmutableListAsync();

        public async Task<ChallengeEntry> AddToDb(ChallengeEntry challengeEntry)
        {
            await _dbContext.ChallengeEntries.AddAsync(challengeEntry);
            await _dbContext.SaveChangesAsync();

            return challengeEntry;
        }

        public async Task<ChallengeEntry> UpdateStatusInDb(
            int challengeEntryId, ChallengeStatus newStatus, DateTime timestamp)
        {
            var challengeBeforeUpdate = await _dbContext.ChallengeEntries.FindAsync(challengeEntryId);
            // if (challengeToUpdate == null)
            // TODO: return OperationResult.NotFound;
            _dbContext.Entry(challengeBeforeUpdate).State = EntityState.Detached;

            var updatedChallenge = challengeBeforeUpdate with { Date = timestamp, Status = newStatus };
            _dbContext.ChallengeEntries.Update(updatedChallenge);
            await _dbContext.SaveChangesAsync();

            return updatedChallenge;
        }
    }
}
