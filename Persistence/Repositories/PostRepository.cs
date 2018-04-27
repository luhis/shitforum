using System;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Persistence.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ForumContext client;

        public PostRepository(ForumContext client)
        {
            this.client = client;
        }

        Task IPostRepository.Add(Post post)
        {
            this.client.Add(post);
            return this.client.SaveChangesAsync();
        }

        IQueryable<Post> IPostRepository.GetAll()
        {
            return this.client.Posts;
        }

        async Task<Option<Post>> IPostRepository.GetById(Guid postId)
        {
            var r = await this.client.Posts.Where(a => a.Id == postId).SingleOrDefaultAsync();
            return r == null ? Option.None<Post>() : Option.Some(r);
        }

        Task<int> IPostRepository.GetThreadPostCount(Guid threadId)
        {
            return this.client.Posts.Where(a => a.ThreadId == threadId).CountAsync();
        }

        Task IPostRepository.Delete(Post post)
        {
            this.client.Posts.Remove(post);
            return this.client.SaveChangesAsync();
        }
    }
}