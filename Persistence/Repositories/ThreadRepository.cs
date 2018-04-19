using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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

        async Task IThreadRepository.Add(Thread thread)
        {
            this.client.Add(thread);
            await this.client.SaveChangesAsync();
        }

        Task<Thread> IThreadRepository.GetById(Guid threadId)
        {
            return this.client.Threads.Where(a => a.Id == threadId).SingleAsync();
        }

        IQueryable<Thread> IThreadRepository.GetAll()
        {
            return this.client.Threads;
        }
    }
}