using Optional;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IBoardRepository
    {
        Task<IEnumerable<Board>> GetAll();
        Task<Option<Board>> GetById(Guid boardId);
        Task<Option<Board>> GetByKey(string key);
    }
}
