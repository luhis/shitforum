using System.Net;
using Domain;

namespace ShitForum.Hasher
{
    public interface IIpHasher
    {
        IpHash Hash(IPAddress ip);
    }
}