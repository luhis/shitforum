using System.Net;
using Domain.IpHash;

namespace ShitForum.Hasher
{
    public interface IIpHasher
    {
        IIpHash Hash(IPAddress ip);
    }
}
