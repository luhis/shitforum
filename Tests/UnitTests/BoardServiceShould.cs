using System;
using System.Collections.Generic;
using System.Threading;
using Domain;
using Domain.Repositories;
using Moq;
using Services;
using Services.Interfaces;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests
{
    public class BoardServiceShould
    {
        private readonly MockRepository repo;
        private readonly IBoardService fs;
        private readonly Mock<IBoardRepository> br;
        private readonly CancellationToken ct = CancellationToken.None;

        public BoardServiceShould()
        {
            this.repo = new MockRepository(MockBehavior.Strict);
            this.br = repo.Create<IBoardRepository>();
            this.fs = new BoardService(this.br.Object);
        }

        [Fact]
        public void GetAll()
        {
            this.br.Setup(a => a.GetAll(ct))
                .ReturnsT(new List<Board>() { new Board(Guid.NewGuid(), "a", "b") });
            var r = this.fs.GetAll(ct).Result;
        }
    }
}
