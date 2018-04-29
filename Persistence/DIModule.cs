using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence
{
    public static class DIModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddSingleton<IThreadRepository, ThreadRepository>();
            services.AddSingleton<IBoardRepository, BoardRepository>();
            services.AddSingleton<IPostRepository, PostRepository>();
            services.AddSingleton<IBannedIpRepository, BannedIpRepository>();
            services.AddSingleton<IBannedImageRepository, BannedImageRepository>();
            services.AddSingleton<IFileRepository, FileRepository>();
            services.AddSingleton<ForumContext>();
        }
    }
}
