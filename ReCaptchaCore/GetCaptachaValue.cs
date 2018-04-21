using Microsoft.AspNetCore.Http;

namespace ReCaptchaCore
{
    public class GetCaptchaValue : IGetCaptchaValue
    {
        string IGetCaptchaValue.Get(HttpRequest req) => req.HttpContext.Request.Form["g-recaptcha-response"];
    }
}
