using System;
using System.Net;
using Domain.IpHash;

namespace ShitForum.Hasher
{
    public class SecureHasherHasher : IIpHasher
    {
        private readonly Guid salt;

        public SecureHasherHasher(Guid salt)
        {
            this.salt = salt;
        }

        IIpHash IIpHasher.Hash(IPAddress ip) => new IpHash(Repeater.DoXTimes(salt.ToString() + ip, Sha256Hasher.Hash, 10));
    }

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
