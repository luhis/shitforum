using Microsoft.Extensions.DependencyInjection;

namespace Cookies
{
    public static class DIModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddSingleton<ICookieStorage, CookieStorage>();
        }
    }
}
