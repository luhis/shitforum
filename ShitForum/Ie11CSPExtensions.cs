using System;
using Microsoft.AspNetCore.Builder;

namespace ShitForum
{
    public static class Ie11CspExtensions
    {
        private const string BaseName = "Content-Security-Policy";

        public static void UseXContentSecurityPolicy(this IApplicationBuilder app)
        {
            app.Use((context, next) =>
            {
                if (!context.Response.Headers.ContainsKey(BaseName))
                {
                    throw new Exception($"{BaseName} header must be set before adding IE11 compatibility");
                }

                var existing = context.Response.Headers[BaseName];
                context.Response.Headers[$"X-{BaseName}"] = existing;

                return next.Invoke();
            });
        }
    }
}
