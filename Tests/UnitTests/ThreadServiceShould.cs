using Domain;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using Optional;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Services.Interfaces;
using UnitTests.Tooling;
using Xunit;
using Thread = Domain.Thread;

namespace UnitTests
{
    public class ThreadServiceShould
    {
        private readonly MockRepository repo;
        private readonly IThreadService ts;

        private readonly Mock<IThreadRepository> threadRepository;
        private readonly Mock<IPostRepository> postRepository;
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<IFileRepository> fileRepository;
        private readonly CancellationToken ct = CancellationToken.None;

        public ThreadServiceShould()
        {
            this.repo = new MockRepository(MockBehavior.Strict);
            this.threadRepository = repo.Create<IThreadRepository>();
            this.postRepository = repo.Create<IPostRepository>();
            this.boardRepository = repo.Create<IBoardRepository>();
            this.fileRepository = repo.Create<IFileRepository>();

            ts = new ThreadService(this.threadRepository.Object, this.postRepository.Object, this.fileRepository.Object, this.boardRepository.Object);
        }

        [Fact]
        public void GetThread()
        {
            var threadId = Guid.NewGuid();
            var boardId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            this.threadRepository.Setup(a => a.GetAll()).Returns(new TestAsyncEnumerable<Thread>(new Thread[] { new Thread(threadId, boardId, "subject"),  }));
            this.threadRepository.Setup(a => a.GetById(threadId, ct)).ReturnsT(Option.Some(new Thread(threadId, boardId, "my thread")));
            this.postRepository.Setup(a => a.GetAll()).Returns(new TestAsyncEnumerable<Post>(new[] { new Post(postId, threadId, DateTime.UtcNow, "name", "comment", false, "") }));
            this.boardRepository.Setup(a => a.GetById(boardId, ct)).ReturnsT(Option.Some(new Board(boardId, "b", "bbb")));
            this.fileRepository.Setup(a => a.GetPostFile(postId, ct)).ReturnsT(Option.Some(new File(Guid.NewGuid(), "file.jpg", new byte[0], new byte[0], "jpg")));

            var r = ts.GetThread(threadId, 100, ct).Result;

            r.Should().NotBeNull();
            r.HasValue.Should().BeTrue();
            repo.VerifyAll();
        }

        [Fact]
        public void NotGetThreadWhenBoardNotFound()
        {
            var threadId = Guid.NewGuid();
            var boardId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            this.threadRepository.Setup(a => a.GetById(threadId, ct)).ReturnsT(Option.Some(new Thread(threadId, boardId, "my thread")));
            this.postRepository.Setup(a => a.GetAll()).Returns(new TestAsyncEnumerable<Post>(new Post[] { new Post(postId, threadId, DateTime.UtcNow, "name", "comment", false, "") }));
            this.boardRepository.Setup(a => a.GetById(boardId, ct)).ReturnsT(Option.None<Board>());
            var r = ts.GetThread(threadId, 100, ct).Result;
            r.Should().NotBeNull();
            r.HasValue.Should().BeFalse();
            repo.VerifyAll();
        }

        [Fact]
        public void GetOrderedCatalogThreads()
        {
            var boardId = Guid.NewGuid();
            this.postRepository.Setup(a => a.GetAll()).Returns(new Post[] { }.AsQueryable());
            this.boardRepository.Setup(a => a.GetByKey("bee", ct)).ReturnsT(Option.Some(new Board(boardId, "b", "bee")));
            this.threadRepository.Setup(a => a.GetAll()).Returns(new TestAsyncEnumerable<Thread>(new List<Thread> { new Thread(Guid.NewGuid(), boardId, "subject") }));
            var r = ts.GetOrderedCatalogThreads("bee", ct).Result;
            r.Should().NotBeNull();

            repo.VerifyAll();
        }

        [Fact]
        public void GetOrderedThreads()
        {
            var boardId = Guid.NewGuid();
            this.postRepository.Setup(a => a.GetAll()).Returns(new Post[] { }.AsQueryable());
            this.boardRepository.Setup(a => a.GetByKey("bee", ct)).ReturnsT(Option.Some(new Board(boardId, "b", "bbb")));
            var thread = new Thread(Guid.NewGuid(), boardId, "subject");
            this.threadRepository.Setup(a => a.GetAll()).
                Returns(new TestAsyncEnumerable<Thread>(new List<Thread> { thread }));
            var r = ts.GetOrderedThreads("bee", Option.None<string>(), 100, 1, ct).Result;
            r.Should().NotBeNull();

            repo.VerifyAll();
        }

        [Fact]
        public void GetOrderedThreadsFiltered()
        {
            var boardId = Guid.NewGuid();
            this.postRepository.Setup(a => a.GetAll()).Returns(new Post[] { }.AsQueryable());
            this.boardRepository.Setup(a => a.GetByKey("bee", ct)).ReturnsT(Option.Some(new Board(boardId, "b", "bbb")));
            this.threadRepository.Setup(a => a.GetAll()).Returns(new TestAsyncEnumerable<Thread>(new List<Thread> {new Thread(Guid.NewGuid(), boardId, "subject")}));
            var r = ts.GetOrderedThreads("bee", Option.Some("matt"), 100, 1, ct).Result;
            r.Should().NotBeNull();

            repo.VerifyAll();
        }
    }
}
