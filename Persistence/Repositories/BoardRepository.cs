using System;
using System.Collections.Generic;
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

        Task<IReadOnlyList<Board>> IBoardRepository.GetAll()
        {
            return client.Boards.ToReadOnlyAsync();
        }

        Task<Option<Board>> IBoardRepository.GetById(Guid boardId)
        {
            return client.Boards.SingleOrNone(a => a.Id == boardId);
        }

        Task<Option<Board>> IBoardRepository.GetByKey(string key)
        {
            return client.Boards.SingleOrNone(a => a.BoardKey == key);
        }
    }
}
