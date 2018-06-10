using System.Net;
using Domain.IpHash;

namespace Hashers
{
    public interface IIpHasher
    {
        IIpHash Hash(IPAddress ip);
    }
}
