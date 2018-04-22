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
    public class ThreadRepository : IThreadRepository
    {
        private readonly ForumContext client;

        public ThreadRepository(ForumContext client)
        {
            this.client = client;
        }

        async Task<IEnumerable<Thread>> IThreadRepository.GetAll(Guid boardId)
        {
            return (await this.client.Threads.Where(a => a.BoardId == boardId).ToListAsync()).AsEnumerable();
        }

        Task IThreadRepository.Add(Thread thread)
        {
            this.client.Add(thread);
            return this.client.SaveChangesAsync();
        }

        async Task<Option<Thread>> IThreadRepository.GetById(Guid threadId)
        {
            var r = await this.client.Threads.Where(a => a.Id == threadId).SingleOrDefaultAsync();
            return r == null ? Option.None<Thread>(): Option.Some(r);
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