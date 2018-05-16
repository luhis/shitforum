using Optional;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IThreadRepository
    {
        Task Add(Thread thread);

        Task<Option<Thread>> GetById(Guid threadId, CancellationToken cancellationToken);

        IQueryable<Thread> GetAll();
        Task Delete(Thread thread);
    }
}
