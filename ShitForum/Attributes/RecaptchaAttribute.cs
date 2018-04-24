using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ReCaptchaCore;
using ShitForum.GetIp;

namespace ShitForum.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RecaptchaAttribute : Attribute, IAsyncPageFilter
    {
        Task IAsyncPageFilter.OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) => Task.CompletedTask;

        async Task IAsyncPageFilter.OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (string.Equals(context.HttpContext.Request.Method, "POST", StringComparison.InvariantCultureIgnoreCase))
            {
                var getCaptchaValue = context.HttpContext.RequestServices.GetService<IGetCaptchaValue>();
                var recaptchaVerifier = context.HttpContext.RequestServices.GetService<IRecaptchaVerifier>();
                var getIp = context.HttpContext.RequestServices.GetService<IGetIp>();

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