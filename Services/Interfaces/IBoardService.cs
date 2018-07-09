using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Services.Interfaces
{
    public interface IBoardService
    {
        Task<IReadOnlyList<Board>> GetAll(CancellationToken cancellationToken);

        Task Delete(Guid boardId, CancellationToken cancellationToken);

        Task Add(Guid boardId, string boardName, string boardKey);
    }
}
