using System;
using System.Collections.Generic;
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

        async Task<IEnumerable<Post>> IPostRepository.GetAll(Guid threadId)
        {
            return (await this.client.Posts.Where(a => a.ThreadId == threadId).ToListAsync()).AsEnumerable();
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

        Task<Post> IPostRepository.GetFirstPost(Guid threadId)
        {
            return this.client.Posts.Where(a => a.ThreadId == threadId).OrderBy(a => a.Created).FirstAsync();
        }

        Task<int> IPostRepository.GetThreadPostCount(Guid threadId)
        {
            return this.client.Posts.Where(a => a.ThreadId == threadId).CountAsync();
        }

        async Task IPostRepository.Delete(Guid postId)
        {
            var post = await this.client.Posts.Where(a => a.Id == postId).SingleAsync();
            this.client.Posts.Remove(post);
            await this.client.SaveChangesAsync();
        }
    }
}