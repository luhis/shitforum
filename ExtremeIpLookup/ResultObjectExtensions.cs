using System;

namespace ExtremeIpLookup
{
    public static class ResultObjectExtensions
    {
        public static T Match<T>(this ResultObject o, Func<ResultObject, T> onSuccess, Func<string, T> onFail)
        {
            if (string.Equals(o.Status, "success", StringComparison.InvariantCultureIgnoreCase))
            {
                return onSuccess(o);
            }
            else
            {
                return onFail(o.Status);
            }
        }
    }

}
