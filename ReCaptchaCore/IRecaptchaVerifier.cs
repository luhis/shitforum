using System.Net;
using System.Threading.Tasks;

namespace ReCaptchaCore
{
    public interface IRecaptchaVerifier
    {
        Task<bool> IsValid(string response, IPAddress ip);
    }
}
