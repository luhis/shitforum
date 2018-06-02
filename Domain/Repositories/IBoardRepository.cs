using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IBoardRepository
    {
        [Pure]
        Task<IReadOnlyList<Board>> GetAll(CancellationToken cancellationToken);

        [Pure]
        Task<Option<Board>> GetById(Guid boardId, CancellationToken cancellationToken);

        [Pure]
        Task<Option<Board>> GetByKey(string key, CancellationToken cancellationToken);

        [Pure]
        Task Remove(Board board);

        [Pure]
        Task Add(Board board);
    }
}
