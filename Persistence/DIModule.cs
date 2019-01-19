using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence
{
    public static class DIModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddScoped<IThreadRepository, ThreadRepository>();
            services.AddScoped<IBoardRepository, BoardRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IBannedIpRepository, BannedIpRepository>();
            services.AddScoped<IBannedImageRepository, BannedImageRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
            services.AddDbContext<ForumContext>();
        }
    }
}
