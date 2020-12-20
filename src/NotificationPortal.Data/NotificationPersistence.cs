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

        public async Task AddNotification(int challengeEntryId, ChallengeNotification challengeNotification)
        {
            var challengeNotificationToSave = challengeNotification with { ChallengeEntryId = challengeEntryId };

            await _dbContext.Notifications.AddAsync(challengeNotificationToSave);
            await _dbContext.SaveChangesAsync();
        }
    }
}
