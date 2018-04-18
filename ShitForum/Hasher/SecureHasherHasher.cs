using System;
using System.Net;
using Domain;

namespace ShitForum.Hasher
{
    public class SecureHasherHasher : IIpHasher
    {
        private readonly Guid salt;

        public SecureHasherHasher(Guid salt)
        {
            this.salt = salt;
        }

        IpHash IIpHasher.Hash(IPAddress ip) => new IpHash(Sha256Hasher.Hash(salt.ToString() + ip));
    }
}