using Microsoft.AspNetCore.Http;
using System.Net;

namespace ShitForum
{
    public class GetIp : IGetIp
    {
        IPAddress IGetIp.GetIp(HttpRequest req) => req.HttpContext.Connection.RemoteIpAddress;
    }
}