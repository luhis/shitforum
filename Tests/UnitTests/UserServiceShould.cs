using System;
using System.Collections.Generic;
using System.Threading;
using Domain;
using Domain.IpHash;
using Domain.Repositories;
using Moq;
using Optional;
using Services;
using Services.Interfaces;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests
{
    public class UserServiceShould
    {
        private readonly MockRepository repo;
        private readonly IUserService ts;

        private readonly Mock<IBannedIpRepository> bannedIpRepository;
        private readonly Mock<IPostRepository> postRepository;
        private readonly CancellationToken ct = CancellationToken.None;
        private const string HashValue = "aaaabbbbccccddddeeeeffffgggghhhhiiiijjjjkkk=";

        public UserServiceShould()
        {
            this.repo = new MockRepository(MockBehavior.Strict);
            this.bannedIpRepository = repo.Create<IBannedIpRepository>();
            this.postRepository = repo.Create<IPostRepository>();

            this.ts = new UserService(bannedIpRepository.Object, postRepository.Object);
        }

        [Fact]
        public void GetAllBans()
        {
            bannedIpRepository.Setup(a => a.GetAll(ct)).ReturnsT(new List<BannedIp>() { new BannedIp(Guid.NewGuid(), "aaa", "shitposter", DateTime.MinValue.AddSeconds(1)) });

            var r = this.ts.GetAllBans(ct).Result;
            this.repo.VerifyAll();
        }

        [Fact]
        public void GetHashForPost()
        {
            var threadId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            
            this.postRepository.Setup(a => a.GetById(postId, ct)).ReturnsT(Option.Some(new Post(postId, threadId, DateTime.UtcNow, "name", "comment", false, HashValue)));
            
            var r = this.ts.GetHashForPost(postId, ct).Result;
            this.repo.VerifyAll();
        }

        [Fact]
        public void BanUser()
        {
            var hash = new IpHash(HashValue);
            var expiry = DateTime.UtcNow;
            this.bannedIpRepository.Setup(a => a.Ban(hash, "", expiry)).Returns(System.Threading.Tasks.Task.CompletedTask);
            this.ts.BanUser(hash, "", expiry).Wait();
            this.repo.VerifyAll();
        }
    }
}
