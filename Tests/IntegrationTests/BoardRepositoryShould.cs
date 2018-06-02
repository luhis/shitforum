using System.Linq;
using System.Threading;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;
using Xunit;

namespace IntegrationTests
{
    public class BoardRepositoryShould
    {
        private readonly IBoardRepository boards;

        public BoardRepositoryShould()
        {
            var cf = new ForumContext(new DbContextOptionsBuilder<ForumContext>()
                .UseSqlite("Data Source=../ShitForum.db").Options);
            this.boards = new BoardRepository(cf);
        }

        [Fact]
        public void Test()
        {
            var r = boards.GetAll(CancellationToken.None).Result;
            r.Should().NotBeNull();
            var arr = r.ToArray();
            arr.Should().NotBeNull();
        }
    }
}
