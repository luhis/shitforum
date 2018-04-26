using Domain;
using Domain.Repositories;
using Moq;
using Optional;
using Services;
using System;
using System.Threading.Tasks;
using Domain.IpHash;
using Services.Dtos;
using Xunit;
using FluentAssertions;
using Services.Results;

namespace UnitTests
{
    public class PostServiceShould
    {
        private readonly MockRepository repo = new MockRepository(MockBehavior.Strict);
        private readonly IPostService ps;

        private readonly Mock<IThreadRepository> threadRepository;
        private readonly Mock<IPostRepository> postRepository;
        private readonly Mock<IFileRepository> fileRepository;
        private readonly Mock<IBannedIpRepository> bannedIpRepository;

        public PostServiceShould()
        {
            this.threadRepository = repo.Create<IThreadRepository>();
            this.postRepository = repo.Create<IPostRepository>();
            this.fileRepository = repo.Create<IFileRepository>();
            this.bannedIpRepository = repo.Create<IBannedIpRepository>();

            this.ps = new PostService(this.postRepository.Object, this.fileRepository.Object, this.threadRepository.Object, this.bannedIpRepository.Object);
        }

        [Fact]
        public void Add()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            var ip = new IpHash("127.0.0.1");

            this.fileRepository.Setup(a => a.GetImageCount(threadId)).Returns(Task.FromResult(1));
            this.postRepository.Setup(a => a.GetThreadPostCount(threadId)).Returns(Task.FromResult(1));
            this.bannedIpRepository.Setup(a => a.IsBanned(ip)).Returns(Task.FromResult(false));
            this.postRepository.Setup(a => a.Add(It.IsAny<Domain.Post>())).Returns(Task.CompletedTask);
            this.ps.Add(postId, threadId, new TripCodedName("Matt"), "comment", false, ip, Option.None<File>()).Wait();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AddFailTooManyPosts()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            var ip = new IpHash("127.0.0.1");

            this.fileRepository.Setup(a => a.GetImageCount(threadId)).Returns(Task.FromResult(1));
            this.postRepository.Setup(a => a.GetThreadPostCount(threadId)).Returns(Task.FromResult(200));
            this.bannedIpRepository.Setup(a => a.IsBanned(ip)).Returns(Task.FromResult(false));
            var r = this.ps.Add(postId, threadId, new TripCodedName("Matt"), "comment", false, ip, Option.None<File>()).Result;

            r.AsT3.Should().NotBeNull();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AddFailTooManyImages()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            var ip = new IpHash("127.0.0.1");

            this.fileRepository.Setup(a => a.GetImageCount(threadId)).Returns(Task.FromResult(200));
            this.bannedIpRepository.Setup(a => a.IsBanned(ip)).Returns(Task.FromResult(false));
            var r = this.ps.Add(postId, threadId, new TripCodedName("Matt"), "comment", false, ip, Option.None<File>()).Result;

            r.AsT2.Should().NotBeNull();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AddThread()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            var boardId = Guid.NewGuid();
            var ip = new IpHash("127.0.0.1");
            
            this.bannedIpRepository.Setup(a => a.IsBanned(ip)).Returns(Task.FromResult(false));
            this.postRepository.Setup(a => a.Add(It.IsAny<Domain.Post>())).Returns(Task.CompletedTask);
            this.threadRepository.Setup(a => a.Add(It.IsAny<Domain.Thread>())).Returns(Task.CompletedTask);

            var r = this.ps.AddThread(postId, threadId, boardId, "subject", new TripCodedName("Matt"), "comment", false, ip, Option.None<File>()).Result;
            r.IsT0.Should().BeTrue();

            this.repo.VerifyAll();
        }

        [Fact]
        public void NotAddThreadWhenBanned()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            var boardId = Guid.NewGuid();
            var ip = new IpHash("127.0.0.1");

            this.bannedIpRepository.Setup(a => a.IsBanned(ip)).Returns(Task.FromResult(true));
            var r = this.ps.AddThread(postId, threadId, boardId, "subject", new TripCodedName("Matt"), "comment", false, ip, Option.None<File>()).Result;
            r.IsT1.Should().BeTrue();

            this.repo.VerifyAll();
        }
    }
}