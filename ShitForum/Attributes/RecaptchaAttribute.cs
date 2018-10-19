using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using ReCaptchaCore;
using ShitForum.GetIp;

namespace ShitForum.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RecaptchaAttribute : Attribute, IAsyncPageFilter
    {
        private readonly IRecaptchaVerifier recaptchaVerifier;
        private readonly IGetCaptchaValue getCaptchaValue;
        private readonly IGetIp getIp;

        public RecaptchaAttribute(IRecaptchaVerifier recaptchaVerifier, IGetCaptchaValue getCaptchaValue, IGetIp getIp)
        {
            this.recaptchaVerifier = recaptchaVerifier;
            this.getCaptchaValue = getCaptchaValue;
            this.getIp = getIp;
        }

        Task IAsyncPageFilter.OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) => Task.CompletedTask;

        async Task IAsyncPageFilter.OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (string.Equals(context.HttpContext.Request.Method, "POST", StringComparison.InvariantCultureIgnoreCase))
            {
                var ip = getIp.GetIp(context.HttpContext.Request);
                var recaptcha = getCaptchaValue.Get(context.HttpContext.Request);
                if (!await recaptchaVerifier.IsValid(recaptcha, ip))
                {
                    context.ModelState.AddModelError(string.Empty, "Recaptcha is invalid");
                }
            }

            await next();
        }
    }
}
