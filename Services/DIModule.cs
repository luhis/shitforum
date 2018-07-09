using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces;

namespace Services
{
    public static class DIModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddSingleton<IThreadService, ThreadService>();
            services.AddSingleton<IPostService, PostService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IBoardService, BoardService>();
            services.AddSingleton<IAnalyticsService, AnalyticsService>();
        }
    }
}
