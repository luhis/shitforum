using System;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Optional;

namespace Persistence.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ForumContext client;
        private readonly ILogger<PostRepository> logger;

        public PostRepository(ForumContext client, ILogger<PostRepository> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        async Task IPostRepository.Add(Post post)
        {
            this.client.Add(post);
            try
            {
                await this.client.SaveChangesAsync();
            }
            catch(Exception e)
            {
                logger.LogError(e, "Error saving outer: " + e.Message);
                logger.LogError(e, "Error saving inner: " + e.InnerException.Message);
                throw;
            }
        }

        IQueryable<Post> IPostRepository.GetAll()
        {
            return this.client.Posts;
        }

        Task<Option<Post>> IPostRepository.GetById(Guid postId)
        {
            return this.client.Posts.SingleOrNone(a => a.Id == postId);
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
