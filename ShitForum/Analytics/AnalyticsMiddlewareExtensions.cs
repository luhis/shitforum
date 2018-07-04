using Microsoft.AspNetCore.Builder;

namespace ShitForum.Analytics
{
    public static class AnalyticsMiddlewareExtensions
    {
        public static IApplicationBuilder UseAnalyticsMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AnalyticsMiddleware>();
        }
    }
}
