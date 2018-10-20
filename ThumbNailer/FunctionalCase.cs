using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

namespace ThumbNailer
{
    public static class FunctionalCase
    {
        public static TR Exec<T, TI, TR>(IReadOnlyDictionary<Option<T>, Func<TI, TR>> options, Func<Option<T>, T, bool> compare, T input, TI data)
        {
            var found = options.Where(a => compare(a.Key, input)).Select(Option.Some).FirstOrDefault();
            return found.Match(a => a.Value(data), () => options.Single(a => !a.Key.HasValue).Value(data));
        }
    }
}
