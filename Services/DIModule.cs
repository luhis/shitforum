using Microsoft.Extensions.DependencyInjection;

namespace Services
{
    public static class DIModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddSingleton<IThreadService, ThreadService>();
            services.AddSingleton<IPostService, PostService>();
        }
    }
}
