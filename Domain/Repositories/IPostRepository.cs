using Optional;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IPostRepository
    {
        Task Add(Post post);

        Task<Option<Post>> GetById(Guid postId);

        Task<int> GetThreadPostCount(Guid threadId);

        IQueryable<Post> GetAll();

        Task Delete(Post post);
    }
}