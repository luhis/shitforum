using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces;
using Services.Services;

namespace Services
{
    public static class DIModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddScoped<IThreadService, ThreadService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IBoardService, BoardService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
        }
    }
}
