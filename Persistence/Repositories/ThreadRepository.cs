using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Repositories;
using Optional;
using Thread = Domain.Thread;

namespace Persistence.Repositories
{
    public class ThreadRepository : IThreadRepository
    {
        private readonly ForumContext client;

        public ThreadRepository(ForumContext client)
        {
            this.client = client;
        }

        Task IThreadRepository.Add(Thread thread)
        {
            this.client.Add(thread);
            return this.client.SaveChangesAsync();
        }

        Task<Option<Thread>> IThreadRepository.GetById(Guid threadId, CancellationToken cancellationToken)
        {
            return this.client.Threads.SingleOrNone(a => a.Id == threadId, cancellationToken);
        }

        IQueryable<Thread> IThreadRepository.GetAll()
        {
            return this.client.Threads;
        }

        Task IThreadRepository.Delete(Thread thread)
        {
            this.client.Threads.Remove(thread);
            return this.client.SaveChangesAsync();
        }
    }
}
