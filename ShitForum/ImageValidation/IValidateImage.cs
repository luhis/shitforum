using Domain;
using Optional;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ShitForum.ImageValidation
{
    public interface IValidateImage
    {
        Task ValidateAsync(Option<byte[]> data, IPAddress ip, IpHash hash, Action<string> addError);
    }
}