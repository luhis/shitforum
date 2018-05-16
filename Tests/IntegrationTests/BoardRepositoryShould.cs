using System.Linq;
using System.Threading;
using Domain.Repositories;
using FluentAssertions;
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
            var cf = new ForumContext(new Config("../ShitForum.db"));
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
