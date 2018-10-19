using System;
using Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShitForum.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CookieAuthAttribute : Attribute, IPageFilter
    {
        private readonly ICookieStorage cookieStorage;
        private readonly AdminSettings adminSettings;

        public CookieAuthAttribute(ICookieStorage cookieStorage, AdminSettings adminSettings)
        {
            this.cookieStorage = cookieStorage;
            this.adminSettings = adminSettings;
        }

        void IPageFilter.OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
        }

        void IPageFilter.OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var cookie = cookieStorage.ReadAdmin(context.HttpContext.Request);
            cookie.Match(some =>
            {
                if (!adminSettings.IsValid(some).HasValue)
                {
                    context.Result = new ForbidResult();
                }
            }, () => context.Result = new ForbidResult());
        }

        void IPageFilter.OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
        }
    }
}
