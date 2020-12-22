using System;
using System.Collections.Generic;
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

        public async Task<List<ChallengeEntry>> GetAllFromDb()
        {
            // TODO: Remove
            await Task.Delay(5000);
            return await _dbContext.ChallengeEntries.ToListAsync();
        }

        public async Task<ChallengeEntry> SaveToDb(ChallengeEntry challengeEntry)
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
