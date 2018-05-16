using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Optional;

namespace Persistence.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly ForumContext client;

        public BoardRepository(ForumContext client)
        {
            this.client = client;
        }

        Task<IReadOnlyList<Board>> IBoardRepository.GetAll(CancellationToken cancellationToken)
        {
            return client.Boards.ToReadOnlyAsync(cancellationToken);
        }

        Task<Option<Board>> IBoardRepository.GetById(Guid boardId, CancellationToken cancellationToken)
        {
            return client.Boards.SingleOrNone(a => a.Id == boardId, cancellationToken);
        }

        Task<Option<Board>> IBoardRepository.GetByKey(string key, CancellationToken cancellationToken)
        {
            return client.Boards.SingleOrNone(a => a.BoardKey == key, cancellationToken);
        }
    }
}
