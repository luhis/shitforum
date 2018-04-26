using System.Net;
using Domain.IpHash;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ShitForum.BannedImageLogger
{
    public interface IBannedImageLogger
    {
        void Log(ModelStateEntry modelStateEntry, IPAddress ip, IIpHash ipHash);
    }
}