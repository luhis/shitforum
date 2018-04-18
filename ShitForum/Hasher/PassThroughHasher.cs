using System.Net;
using Domain;

namespace ShitForum.Hasher
{
    public class PassThroughHasher : IIpHasher
    {
        IpHash IIpHasher.Hash(IPAddress ip) => new IpHash(ip.ToString());
    }
}