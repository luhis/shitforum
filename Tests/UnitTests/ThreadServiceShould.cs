using Domain;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using Optional;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.Tooling;
using Xunit;

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
            this.threadRepository.Setup(a => a.GetById(threadId)).Returns(Task.FromResult(Option.Some(new Thread(threadId, boardId, "my thread"))));
            this.postRepository.Setup(a => a.GetAll()).Returns(new TestAsyncEnumerable<Post>(new Domain.Post[] { new Domain.Post(postId, threadId, DateTime.UtcNow, "name", "comment", false, "") }));
            this.boardRepository.Setup(a => a.GetById(boardId)).Returns(Task.FromResult(Option.Some(new Board(boardId, "b", "bbb"))));
            this.fileRepository.Setup(a => a.GetPostFile(postId)).Returns(Task.FromResult(Option.Some(new File())));
            var r = ts.GetThread(threadId).Result;
            r.Should().NotBeNull();

            repo.VerifyAll();
        }

        [Fact]
        public void GetOrderedCatalogThreads()
        {
            var boardId = Guid.NewGuid();
            this.postRepository.Setup(a => a.GetAll()).Returns(new Domain.Post[] { }.AsQueryable());
            this.boardRepository.Setup(a => a.GetById(boardId)).Returns(Task.FromResult(Option.Some(new Board(boardId, "b", "bbb"))));
            this.threadRepository.Setup(a => a.GetAll()).Returns(new TestAsyncEnumerable<Thread>(new List<Thread> { new Thread(Guid.NewGuid(), boardId, "subject") }));
            var r = ts.GetOrderedCatalogThreads("bee").Result;
            r.Should().NotBeNull();

            repo.VerifyAll();
        }

        [Fact]
        public void GetOrderedThreads()
        {
            var boardId = Guid.NewGuid();
            this.postRepository.Setup(a => a.GetAll()).Returns(new Domain.Post[] { }.AsQueryable());
            this.boardRepository.Setup(a => a.GetById(boardId)).Returns(Task.FromResult(Option.Some(new Board(boardId, "b", "bbb"))));
            this.threadRepository.Setup(a => a.GetAll()).Returns(new TestAsyncEnumerable<Thread>(new List<Thread> { new Thread(Guid.NewGuid(), boardId, "subject") }));
            var r = ts.GetOrderedThreads("bee", Option.None<string>(), 100, 0).Result;
            r.Should().NotBeNull();

            repo.VerifyAll();
        }

        [Fact]
        public void GetOrderedThreadsFiltered()
        {
            var boardId = Guid.NewGuid();
            this.postRepository.Setup(a => a.GetAll()).Returns(new Domain.Post[] { }.AsQueryable());
            this.boardRepository.Setup(a => a.GetById(boardId)).Returns(Task.FromResult(Option.Some(new Board(boardId, "b", "bbb"))));
            this.threadRepository.Setup(a => a.GetAll()).Returns(new TestAsyncEnumerable<Thread>(new List<Thread> {new Thread(Guid.NewGuid(), boardId, "subject")}));
            var r = ts.GetOrderedThreads("bee", Option.Some("matt"), 100, 0).Result;
            r.Should().NotBeNull();

            repo.VerifyAll();
        }
    }
}
