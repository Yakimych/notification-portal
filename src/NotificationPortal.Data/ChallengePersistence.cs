using System.Threading.Tasks;

namespace NotificationPortal.Data
{
    public class ChallengePersistence
    {
        private readonly ApplicationDbContext _dbContext;

        public ChallengePersistence(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ChallengeEntry> SaveToDb(ChallengeEntry challengeEntry)
        {
            await _dbContext.ChallengeEntries.AddAsync(challengeEntry);
            await _dbContext.SaveChangesAsync();

            return challengeEntry;
        }
    }
}
