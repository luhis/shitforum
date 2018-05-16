using System;
using System.Linq;
using System.Threading;
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

        Task IFileRepository.Add(File file)
        {
            EnsureArg.IsNotNull(file, nameof(file));
            this.client.Files.Add(file);
            return client.SaveChangesAsync();
        }

        Task<int> IFileRepository.GetImageCount(Guid threadId, CancellationToken cancellationToken)
        {
            var posts = client.Posts.Where(a => a.ThreadId == threadId).Select(a => a.Id);

            return client.Files.CountAsync(a => posts.Contains(a.Id), cancellationToken);
        }

        Task<Option<File>> IFileRepository.GetPostFile(Guid postId, CancellationToken cancellationToken)
        {
            return client.Files.SingleOrNone(a => a.Id == postId, cancellationToken);
        }
    }
}
