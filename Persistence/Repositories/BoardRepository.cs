using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
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

        async Task<IEnumerable<Board>> IBoardRepository.GetAll()
        {
            return (await client.Boards.ToListAsync()).AsEnumerable();
        }

        async Task<Option<Board>> IBoardRepository.GetById(Guid boardId)
        {
            var c = await client.Boards.Where(a => a.Id == boardId).SingleOrDefaultAsync();
            return c != null ? Option.Some(c) : Option.None<Board>();
        }

        async Task<Option<Board>> IBoardRepository.GetByKey(string key)
        {
            var c = await client.Boards.Where(a => a.BoardKey == key).SingleOrDefaultAsync();
            return c != null ? Option.Some(c) : Option.None<Board>();
        }
    }
}
