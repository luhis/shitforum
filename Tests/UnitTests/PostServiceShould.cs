using Domain;
using Domain.Repositories;
using Moq;
using Optional;
using Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.IpHash;
using Services.Dtos;
using Xunit;
using FluentAssertions;
using UnitTests.Tooling;
using Thread = Domain.Thread;

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
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly CancellationToken ct = CancellationToken.None;

        public PostServiceShould()
        {
            this.threadRepository = repo.Create<IThreadRepository>();
            this.postRepository = repo.Create<IPostRepository>();
            this.fileRepository = repo.Create<IFileRepository>();
            this.bannedIpRepository = repo.Create<IBannedIpRepository>();
            this.boardRepository = repo.Create<IBoardRepository>();

            this.ps = new PostService(this.postRepository.Object, this.fileRepository.Object,
                this.threadRepository.Object, this.bannedIpRepository.Object, this.boardRepository.Object);
        }

        [Fact]
        public void Add()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            var ip = new IpUnHashed("127.0.0.1");

            this.fileRepository.Setup(a => a.GetImageCount(threadId, ct)).ReturnsT(1);
            this.postRepository.Setup(a => a.GetThreadPostCount(threadId, ct)).ReturnsT(1);
            this.bannedIpRepository.Setup(a => a.IsBanned(ip, ct)).ReturnsT(false);
            this.postRepository.Setup(a => a.Add(It.IsAny<Domain.Post>())).Returns(Task.CompletedTask);
            this.ps.Add(postId, threadId, new TripCodedName("Matt"), "comment", false, ip, Option.None<File>(), ct).Wait();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AddFailTooManyPosts()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            var ip = new IpUnHashed("127.0.0.1");

            this.fileRepository.Setup(a => a.GetImageCount(threadId, ct)).ReturnsT(1);
            this.postRepository.Setup(a => a.GetThreadPostCount(threadId, ct)).ReturnsT(200);
            this.bannedIpRepository.Setup(a => a.IsBanned(ip, ct)).ReturnsT(false);
            var r = this.ps.Add(postId, threadId, new TripCodedName("Matt"), "comment", false, ip, Option.None<File>(), ct).Result;

            r.AsT3.Should().NotBeNull();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AddFailTooManyImages()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            var ip = new IpUnHashed("127.0.0.1");

            this.fileRepository.Setup(a => a.GetImageCount(threadId, ct)).ReturnsT(200);
            this.bannedIpRepository.Setup(a => a.IsBanned(ip, ct)).ReturnsT(false);
            var r = this.ps.Add(postId, threadId, new TripCodedName("Matt"), "comment", false, ip, Option.None<File>(), ct).Result;

            r.AsT2.Should().NotBeNull();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AddThread()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            var boardId = Guid.NewGuid();
            var ip = new IpUnHashed("127.0.0.1");
            
            this.bannedIpRepository.Setup(a => a.IsBanned(ip, ct)).ReturnsT(false);
            this.postRepository.Setup(a => a.Add(It.IsAny<Domain.Post>())).Returns(Task.CompletedTask);
            this.threadRepository.Setup(a => a.Add(It.IsAny<Domain.Thread>())).Returns(Task.CompletedTask);

            var r = this.ps.AddThread(postId, threadId, boardId, "subject", new TripCodedName("Matt"), "comment", false, ip, Option.None<File>(), ct).Result;
            r.IsT0.Should().BeTrue();

            this.repo.VerifyAll();
        }

        [Fact]
        public void NotAddThreadWhenBanned()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            var boardId = Guid.NewGuid();
            var ip = new IpUnHashed("127.0.0.1");

            this.bannedIpRepository.Setup(a => a.IsBanned(ip, ct)).ReturnsT(true);
            var r = this.ps.AddThread(postId, threadId, boardId, "subject", new TripCodedName("Matt"), "comment", false, ip, Option.None<File>(), ct).Result;
            r.IsT1.Should().BeTrue();

            this.repo.VerifyAll();
        }

        [Fact]
        public void GetById()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            var boardId = Guid.NewGuid();
            this.postRepository.Setup(a => a.GetById(postId, ct)).ReturnsT(Option.Some(
                new Post(postId, threadId, DateTime.UtcNow, "matt", "comment", false, "::0")));
            this.threadRepository.Setup(a => a.GetById(threadId, ct)).ReturnsT(Option.Some(new Thread(threadId, boardId, "")));
            this.boardRepository.Setup(a => a.GetById(boardId, ct)).ReturnsT(Option.Some(new Board(boardId, "tezt", "pol")));
            this.fileRepository.Setup(a => a.GetPostFile(postId, ct)).ReturnsT(Option.Some(new File(Guid.NewGuid(), "file.jpg", new byte[0], new byte[0], "jpg")));

            var r = this.ps.GetById(postId, ct).Result;
            r.Should().NotBeNull();
            r.HasValue.Should().BeTrue();
            this.repo.VerifyAll();
        }

        [Fact]
        public void NotGetByIdWhenBoardNotFound()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            var boardId = Guid.NewGuid();
            this.postRepository.Setup(a => a.GetById(postId, ct)).ReturnsT(Option.Some(
                new Post(postId, threadId, DateTime.UtcNow, "matt", "comment", false, "::0")));
            this.threadRepository.Setup(a => a.GetById(threadId, ct)).ReturnsT(Option.Some(new Thread(threadId, boardId, "")));
            this.boardRepository.Setup(a => a.GetById(boardId, ct)).ReturnsT(Option.None<Board>());
            this.fileRepository.Setup(a => a.GetPostFile(postId, ct)).ReturnsT(Option.Some(new File(Guid.NewGuid(), "file.jpg", new byte[0], new byte[0], "jpg")));

            var r = this.ps.GetById(postId, ct).Result;
            r.Should().NotBeNull();
            r.HasValue.Should().BeFalse();
            this.repo.VerifyAll();
        }

        [Fact]
        public void NotGetByIdWhenThreadNotFound()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            this.postRepository.Setup(a => a.GetById(postId, ct)).ReturnsT(Option.Some(
                new Post(postId, threadId, DateTime.UtcNow, "matt", "comment", false, "::0")));
            this.threadRepository.Setup(a => a.GetById(threadId, ct)).ReturnsT(Option.None<Thread>());

            var r = this.ps.GetById(postId, ct).Result;
            r.Should().NotBeNull();
            r.HasValue.Should().BeFalse();
            this.repo.VerifyAll();
        }

        [Fact]
        public void NotGetByIdWhenPostNotFound()
        {
            var postId = Guid.NewGuid();
            this.postRepository.Setup(a => a.GetById(postId, ct)).ReturnsT(Option.None<Post>());

            var r = this.ps.GetById(postId, ct).Result;
            r.Should().NotBeNull();
            r.HasValue.Should().BeFalse();
            this.repo.VerifyAll();
        }

        [Fact]
        public void Delete()
        {
            var postId = Guid.NewGuid();
            var threadId = Guid.NewGuid();
            var p = new Post(postId, threadId, DateTime.UtcNow, "matt", "comment", false, "::0");
            this.postRepository.Setup(a => a.GetById(postId, ct)).ReturnsT(Option.Some(p));
            this.postRepository.Setup(a => a.GetThreadPostCount(threadId, ct)).ReturnsT(200);
            this.postRepository.Setup(a => a.Delete(p)).Returns(Task.CompletedTask);

            var r = this.ps.DeletePost(postId, ct).Result;
            r.Should().BeTrue();
            this.repo.VerifyAll();
        }
    }
}
