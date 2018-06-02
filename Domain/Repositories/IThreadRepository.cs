using Optional;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IThreadRepository
    {
        [Pure]
        Task Add(Thread thread);

        [Pure]
        Task<Option<Thread>> GetById(Guid threadId, CancellationToken cancellationToken);

        [Pure]
        IQueryable<Thread> GetAll();

        [Pure]
        Task Delete(Thread thread);
    }
}
