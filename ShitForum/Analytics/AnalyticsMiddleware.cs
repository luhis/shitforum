using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System;
using Cookies;
using ExtremeIpLookup;
using Services.Interfaces;

namespace ShitForum.Analytics
{
    public class AnalyticsMiddleware
    {
        private readonly RequestDelegate next;

        public AnalyticsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        private static Guid GetThumbPrint(ICookieStorage cs, HttpRequest req, HttpResponse res)
        {
            var thumb = cs.ReadThumbPrint(req);
            return thumb.Match(some => some, () =>
            {
                var newThumb = Guid.NewGuid();
                cs.SetThumbPrint(res, newThumb);
                return newThumb;
            });
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var svc = context.RequestServices.GetService<IAnalyticsService>();
            var ipLookup = context.RequestServices.GetService<IExtremeIpLookup>();
            var ip = context.Connection.RemoteIpAddress;
            var deats = await ipLookup.GetIpDetailsAsync(ip);
            
            await deats.Match(o =>
            {
                var cookies = context.RequestServices.GetService<ICookieStorage>();
                var thumb = GetThumbPrint(cookies, context.Request, context.Response);
                return svc.Add(new Domain.AnalyticsReport(Guid.NewGuid(), DateTime.UtcNow, o.City, thumb.ToString()), CancellationToken.None);
            }, _ => Task.CompletedTask);
            
            await this.next(context);
        }
    }
}
