using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShitForum.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace ShitForum.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CookieAuthAttribute : Attribute, IPageFilter
    {
        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var adminSettings = context.HttpContext.RequestServices.GetService<AdminSettings>();
            var cookieStorage = context.HttpContext.RequestServices.GetService<ICookieStorage>();

            var cookie = cookieStorage.ReadAdmin(context.HttpContext.Request);
            cookie.Match(some =>
            {
                if (!adminSettings.IsValid(some).HasValue)
                {
                    context.Result = new ForbidResult();
                }
                else
                {
                    return;
                }

            }, () => context.Result = new ForbidResult());
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
        }
    }
}
