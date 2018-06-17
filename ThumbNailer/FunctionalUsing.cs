using System;

namespace ThumbNailer
{
    public static class FunctionalUsing
    {
        public static TR Using<T, TR>(Func<T> d, Func<T, TR> func) where T : IDisposable
        {
            using (var disposable = d())
            {
                return func(disposable);
            }
        }

        public static void Using<T>(Func<T> d, Action<T> action) where T : IDisposable
        {
            using (var disposable = d())
            {
                action(disposable);
            }
        }
    }
}
