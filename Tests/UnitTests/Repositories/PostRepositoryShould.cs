using System.Linq;
using Domain.Repositories;
using FluentAssertions;
using Persistence;
using Persistence.Repositories;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests.Repositories
{
    public class PostRepositoryShould
    {
        private static IPostRepository MakeRepo(ForumContext ctx) => new PostRepository(ctx, null);

        [Fact]
        public void GetAll() => RepositoryTooling.RunInConnection(MakeRepo, repo =>
        {
            var r = repo.GetAll().ToList();
            r.Should().NotBeNull();
            r.Count.Should().Be(0);
        });
    }
}
