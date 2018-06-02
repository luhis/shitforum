using Optional;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IPostRepository
    {
        [Pure]
        Task Add(Post post);

        [Pure]
        Task<Option<Post>> GetById(Guid postId, CancellationToken cancellationToken);

        [Pure]
        Task<int> GetThreadPostCount(Guid threadId, CancellationToken cancellationToken);

        [Pure]
        IQueryable<Post> GetAll();

        [Pure]
        Task Delete(Post post);
    }
}
