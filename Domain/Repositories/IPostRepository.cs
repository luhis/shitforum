using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAll(Guid threadId);

        Task Add(Post post);

        Task<Option<Post>> GetById(Guid postId);

        Task<Post> GetFirstPost(Guid threadId);

        Task<int> GetThreadPostCount(Guid threadId);

        IQueryable<Post> GetAll();

        Task Delete(Guid postId);
    }
}