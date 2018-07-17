using System;
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
    public class ThreadRepositoryShould
    {
        private static readonly Guid BId = new System.Guid("1f5f47db-dd27-4b58-9229-4ae72829621e");
        private static IThreadRepository MakeRepo(ForumContext ctx) => new ThreadRepository(ctx);

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
            var threadId = Guid.NewGuid();
            repo.Add(new Domain.Thread(threadId, BId, "subject")).Wait();
            var r = repo.GetById(threadId, CancellationToken.None).Result;
            r.Should().NotBeNull();
            r.HasValue.Should().Be(true);
        });

        [Fact]
        public void Delete() => RepositoryTooling.RunInConnection(MakeRepo, repo =>
        {
            var threadId = Guid.NewGuid();
            var thread = new Domain.Thread(threadId, BId, "subject");
            repo.Add(thread).Wait();
            repo.Delete(thread).Wait();
        });
    }
}
