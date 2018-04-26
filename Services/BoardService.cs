using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;

namespace Services
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRepository boardRepository;

        public BoardService(IBoardRepository boardRepository)
        {
            this.boardRepository = boardRepository;
        }

        Task<IEnumerable<Board>> IBoardService.GetAll()
        {
            return this.boardRepository.GetAll();
        }
    }
}