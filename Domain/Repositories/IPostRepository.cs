using Optional;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IPostRepository
    {
        Task Add(Post post);

        Task<Option<Post>> GetById(Guid postId, CancellationToken cancellationToken);

        Task<int> GetThreadPostCount(Guid threadId, CancellationToken cancellationToken);

        IQueryable<Post> GetAll();

        Task Delete(Post post);
    }
}
