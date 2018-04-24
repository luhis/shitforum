using System.Net;
using Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ShitForum
{
    public interface IBannedImageLogger
    {
        void Log(ModelStateEntry modelStateEntry, IPAddress ip, IpHash ipHash);
    }
}