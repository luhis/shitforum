using System;
using System.Net;
using Domain.IpHash;

namespace Hashers
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
}
