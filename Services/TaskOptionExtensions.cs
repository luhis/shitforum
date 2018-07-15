using System;
using System.Threading.Tasks;
using Optional;

namespace Services
{
    public static class TaskOptionExtensions
    {
        ////public static async Task<Option<TO>> MapAsync<T, TO>(this Task<Option<T>> op, Func<T, Task<TO>> f)
        ////{
        ////    var r = await op;
        ////    Option<TO> x = await r.Match<Option<TO>>(async some =>
        ////    {
        ////        return Option.Some(await f(some));
        ////    }, () =>
        ////    {
        ////        return Task.FromResult(Option.None<TO>());
        ////    });

        ////    return x;
        ////}
        ////public static TO MapAsync<T, TO>(Option<T> option, Func<T, TO> func)
        ////{
        ////    throw new NotImplementedException();
        ////}
        ////

        public static Task MapToTask<T>(this Option<T> o, Func<T, Task> f) => o.Match(f, () => Task.CompletedTask);
    }
}
