using Microsoft.Extensions.DependencyInjection;

namespace ThumbNailer
{
    public static class DIModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddSingleton<IThumbNailer, ThumbNailer>();
        }
    }
}
