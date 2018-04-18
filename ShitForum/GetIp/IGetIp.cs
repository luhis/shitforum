using Microsoft.AspNetCore.Http;
using System.Net;

namespace ShitForum
{
    public interface IGetIp
    {
        IPAddress GetIp(HttpRequest req);
    }
}