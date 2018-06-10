using System.Net;
using Domain.IpHash;

namespace Hashers
{
    public class PassThroughHasher : IIpHasher
    {
        IIpHash IIpHasher.Hash(IPAddress ip) => new IpUnHashed(ip.ToString());
    }
}