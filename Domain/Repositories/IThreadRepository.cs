using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IThreadRepository
    {
        Task<IEnumerable<Thread>> GetAll(Guid boardId);

        Task Add(Thread thread);

        Task<Thread> GetById(Guid threadId);
    }
}