using System;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Persistence.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly ForumContext client;

        public FileRepository(ForumContext client)
        {
            this.client = client;
        }

        async Task IFileRepository.Add(File file)
        {
            EnsureArg.IsNotNull(file, nameof(file));
            this.client.Files.Add(file);
            await client.SaveChangesAsync();
        }

        Task<int> IFileRepository.GetImageCount(Guid threadId)
        {
            var posts = client.Posts.Where(a => a.ThreadId == threadId).Select(a => a.Id);

            return client.Files.CountAsync(a => posts.Contains(a.Id));
        }

        async Task<Option<File>> IFileRepository.GetPostFile(Guid postId)
        {
            var f = await client.Files.SingleOrDefaultAsync(a => a.Id == postId);
            return f != null ? Option.Some(f) : Option.None<File>();
        }
    }
}
