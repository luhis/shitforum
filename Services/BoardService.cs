using System.Collections.Generic;
using System.Threading;
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

        Task<IReadOnlyList<Board>> IBoardService.GetAll(CancellationToken cancellationToken)
        {
            return this.boardRepository.GetAll(cancellationToken);
        }
    }
}
