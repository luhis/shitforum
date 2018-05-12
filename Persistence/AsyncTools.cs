using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Persistence
{
    public static class AsyncTools
    {
        public static async Task<IReadOnlyList<T>> ToReadOnlyAsync<T>(this DbSet<T> set) where T : class
        {
           return (IReadOnlyList<T>) await set.ToListAsync();
        }

        public static async Task<Option<T>> SingleOrNone<T>(this DbSet<T> set, Expression<Func<T, bool>> f) where T : class
        {
            var r = await set.Where(f).SingleOrDefaultAsync();
            return r == null ? Option.None<T>() : Option.Some(r);
        }
    }
}
