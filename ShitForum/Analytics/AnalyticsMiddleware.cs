using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System;
using static ExtremeIpLookup.ResultObjectExtensions;

namespace ShitForum.Analytics
{
    public class AnalyticsMiddleware
    {
        private readonly RequestDelegate next;

        public AnalyticsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var svc = context.RequestServices.GetService<IAnalyticsService>();
            var ip = context.Connection.RemoteIpAddress;
            var deats = await ExtremeIpLookup.ExtremeIpLookup.GetIpDetailsAsync(ip);

            await deats.Match(o =>
            {
                // todo fix the ip storage
                return svc.Add(new Domain.AnalyticsReport(Guid.NewGuid(), DateTime.UtcNow, o.City, ip.ToString()), CancellationToken.None);
            }, _ => Task.CompletedTask);

            // Call the next delegate/middleware in the pipeline
            await this.next(context);
        }
    }
}
