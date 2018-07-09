using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Services.Interfaces;

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

        async Task IBoardService.Delete(Guid boardId, CancellationToken cancellationToken)
        {
            var b = await this.boardRepository.GetById(boardId, cancellationToken);
            await b.Match(this.boardRepository.Remove, () => Task.CompletedTask);
        }

        Task IBoardService.Add(Guid boardId, string boardName, string boardKey)
        {
            return this.boardRepository.Add(new Board(boardId, boardName, boardKey));
        }
    }
}
