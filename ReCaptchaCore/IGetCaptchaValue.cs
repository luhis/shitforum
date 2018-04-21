using Microsoft.AspNetCore.Http;

namespace ReCaptchaCore
{

    public interface IGetCaptchaValue
    {
        string Get(HttpRequest req);
    }
}
