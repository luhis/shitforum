using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System;
using Cookies;
using ExtremeIpLookup;
using Hashers;
using Microsoft.Extensions.Logging;
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

        public async Task InvokeAsync(HttpContext context, IAnalyticsService analyticsService, IExtremeIpLookup ipLookup, ICookieStorage cookies, ILogger<AnalyticsMiddleware> logger)
        {
            var ip = context.Connection.RemoteIpAddress;
            try
            {
                var deats = await ipLookup.GetIpDetailsAsync(ip);

                await deats.Match(o =>
                {
                    var thumb = GetThumbPrint(cookies, context.Request, context.Response);
                    return analyticsService.Add(
                        new Domain.AnalyticsReport(Guid.NewGuid(), DateTime.UtcNow, o.City,
                            Sha256Hasher.Hash(thumb.ToString())), CancellationToken.None);
                }, _ => Task.CompletedTask);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error running analytics");
            }

            await this.next(context);
        }
    }
}
