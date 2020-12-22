using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NotificationPortal.Data
{
    public static class EntityFrameworkExtensions
    {
        public static Task<ImmutableList<T>> ToImmutableListAsync<T>(this DbSet<T> dbSet) where T : class =>
            dbSet.ToListAsync().ContinueWith(r => r.Result.ToImmutableList());
    }
}
