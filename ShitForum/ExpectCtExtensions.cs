using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ShitForum
{
    public static class ExpectCtExtensions
    {
        const int MaxAge = 60 * 60 * 31;
        public static void UseExpectCt(this IApplicationBuilder app)
        {
            app.Use((context, next) =>
            {
                if (context.Request.IsHttps)
                {
                    context.Response.Headers.Append("Expect-CT",
                        $"max-age={MaxAge};");
                }

                return next.Invoke();
            });
        }
    }
}
