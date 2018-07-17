using System.Threading;
using Domain.Repositories;
using FluentAssertions;
using Persistence;
using Persistence.Repositories;
using Tests.UnitTests.Tooling;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests.Repositories
{
    public class BannedImageRepositoryShould
    {
        private static IBannedImageRepository MakeRepo(ForumContext ctx) => new BannedImageRepository(ctx);

        [Fact]
        public void GetAll() => RepositoryTooling.RunInConnection(MakeRepo, repo =>
        {
            var r = repo.GetAll(CancellationToken.None).Result;
            r.Should().NotBeNull();
            r.Count.Should().Be(0);
        });
    }
}
