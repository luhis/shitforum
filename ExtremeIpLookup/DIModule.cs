using Microsoft.Extensions.DependencyInjection;

namespace ExtremeIpLookup
{
    public static class DIModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddSingleton<IExtremeIpLookup, ExtremeIpLookup>();
        }
    }
}
