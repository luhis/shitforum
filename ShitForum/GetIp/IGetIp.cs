using System.Net;
using Microsoft.AspNetCore.Http;

namespace ShitForum.GetIp
{
    public interface IGetIp
    {
        IPAddress GetIp(HttpRequest req);
    }
}