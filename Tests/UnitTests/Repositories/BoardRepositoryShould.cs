using System;
using System.Threading;
using Domain.Repositories;
using FluentAssertions;
using Persistence;
using Persistence.Repositories;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests.Repositories
{
    public class BoardRepositoryShould
    {
        private static readonly Guid BId = new Guid("1f5f47db-dd27-4b58-9229-4ae72829621e");
        private static IBoardRepository MakeRepo(ForumContext ctx) => new BoardRepository(ctx);

        [Fact]
        public void NotFind() => RepositoryTooling.RunInConnection(MakeRepo, repo =>
        {
            var r = repo.GetById(Guid.Empty, CancellationToken.None).Result;
            r.Should().NotBeNull();
            r.HasValue.Should().Be(false);
        });

        [Fact]
        public void Find() => RepositoryTooling.RunInConnection(MakeRepo, repo =>
        {
            var r = repo.GetById(BId, CancellationToken.None).Result;
            r.Should().NotBeNull();
            r.HasValue.Should().Be(true);
        });

        [Fact]
        public void GetByKey() => RepositoryTooling.RunInConnection(MakeRepo, repo =>
        {
            var r = repo.GetByKey("b", CancellationToken.None).Result;
            r.Should().NotBeNull();
            r.HasValue.Should().Be(true);
        });

        [Fact]
        public void GetAll() => RepositoryTooling.RunInConnection(MakeRepo, repo =>
        {
            var r = repo.GetAll(CancellationToken.None).Result;
            r.Should().NotBeNull();
            r.Count.Should().Be(4);
        });
    }
}
