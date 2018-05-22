using Optional;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IBoardRepository
    {
        Task<IReadOnlyList<Board>> GetAll(CancellationToken cancellationToken);

        Task<Option<Board>> GetById(Guid boardId, CancellationToken cancellationToken);

        Task<Option<Board>> GetByKey(string key, CancellationToken cancellationToken);

        Task Remove(Board board);

        Task Add(Board board);
    }
}
