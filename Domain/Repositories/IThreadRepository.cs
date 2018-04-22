using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IThreadRepository
    {
        Task<IEnumerable<Thread>> GetAll(Guid boardId);

        Task Add(Thread thread);

        Task<Option<Thread>> GetById(Guid threadId);

        IQueryable<Thread> GetAll();
        Task Delete(Thread thread);
    }
}