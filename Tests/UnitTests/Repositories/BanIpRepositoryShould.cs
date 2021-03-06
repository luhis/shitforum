﻿using System.Threading;
using Domain.Repositories;
using FluentAssertions;
using Persistence;
using Persistence.Repositories;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests.Repositories
{
    public class BannedIpRepositoryShould
    {
        private static IBannedIpRepository MakeRepo(ForumContext ctx) => new BannedIpRepository(ctx);

        [Fact]
        public void GetAll() => RepositoryTooling.RunInConnection(MakeRepo, repo =>
        {
            var r = repo.GetAll(CancellationToken.None).Result;
            r.Should().NotBeNull();
            r.Count.Should().Be(0);
        });
    }
}
