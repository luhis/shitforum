using System;

namespace Hashers
{
    public static class Repeater
    {
        public static T DoXTimes<T>(T input, Func<T, T> f, int times)
        {
            if (times <= 0)
            {
                return input;
            }
            else
            {
                return DoXTimes(f(input), f, times - 1);
            }
        }
    }
}
