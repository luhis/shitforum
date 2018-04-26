using System.Net;
using Domain.IpHash;

namespace ShitForum.Hasher
{
    public class PassThroughHasher : IIpHasher
    {
        IIpHash IIpHasher.Hash(IPAddress ip) => new IpUnHashed(ip.ToString());
    }
}