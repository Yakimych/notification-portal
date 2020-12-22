using System.Collections.Immutable;
using System.Threading.Tasks;

namespace NotificationPortal.Data
{
    public class NotificationPersistence
    {
        private readonly ApplicationDbContext _dbContext;

        public NotificationPersistence(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ImmutableList<ChallengeNotification>> GetAllFromDb() =>
            _dbContext.Notifications.ToImmutableListAsync();

        public async Task AddToDb(int challengeEntryId, ChallengeNotification challengeNotification)
        {
            var challengeNotificationToSave = challengeNotification with { ChallengeEntryId = challengeEntryId };

            await _dbContext.Notifications.AddAsync(challengeNotificationToSave);
            await _dbContext.SaveChangesAsync();
        }
    }
}
