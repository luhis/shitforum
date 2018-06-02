using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Optional;

namespace Domain.Repositories
{
    public interface IFileRepository
    {
        [Pure]
        Task<Option<File>> GetPostFile(Guid postId, CancellationToken cancellationToken);

        [Pure]
        Task<int> GetImageCount(Guid threadId, CancellationToken cancellationToken);

        [Pure]
        Task Add(File file);
    }
}
