using System;
using System.Threading;
using System.Threading.Tasks;
using Optional;

namespace Domain.Repositories
{
    public interface IFileRepository
    {
        Task<Option<File>> GetPostFile(Guid postId, CancellationToken cancellationToken);

        Task<int> GetImageCount(Guid threadId, CancellationToken cancellationToken);

        Task Add(File file);
    }
}
