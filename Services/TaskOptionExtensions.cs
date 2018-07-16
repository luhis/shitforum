using System;
using System.Threading.Tasks;
using Optional;

namespace Services
{
    public static class TaskOptionExtensions
    {
        public static Task MapToTask<T>(this Option<T> o, Func<T, Task> f) => o.Match(f, () => Task.CompletedTask);

        public static Task<TO> MapToTask<T, TO>(this Option<T> o, Func<T, Task<TO>> f, TO deff)
        {
            return o.Match(f, () => Task.FromResult(deff));
        }

        public static Task<Option<TO>> MapToTask<T, TO>(this Option<T> o, Func<T, Task<TO>> f)
        {
            return o.Match(async a => Option.Some(await f(a)), () => Task.FromResult(Option.None<TO>()));
        }
    }
}
