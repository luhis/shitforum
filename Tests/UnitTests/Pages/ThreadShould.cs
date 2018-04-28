using Moq;
using ShitForum.Hasher;
using ShitForum.Pages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Xunit;
using Microsoft.AspNetCore.Http;
using ShitForum.Models;
using System.Net;
using Optional;
using Services;
using Services.Dtos;
using Services.Results;
using OneOf;
using Domain.IpHash;
using ShitForum;
using ShitForum.BannedImageLogger;
using ShitForum.Cookies;
using ShitForum.GetIp;
using UnitTests.Tooling;

namespace UnitTests.Pages
{
    public class ThreadShould
    {
        private readonly MockRepository repo;
        private readonly Mock<ICookieStorage> cookieStorage;
        private readonly ThreadModel thread;
        private readonly Mock<IGetIp> getIp;
        private readonly Mock<IThreadService> threadService;
        private readonly Mock<IPostService> postService;
        private readonly Mock<IBannedImageLogger> bannedImageLogger;
        private readonly Mock<IIsAdmin> iIsAdmin;

        public ThreadShould()
        {
            var conf = MockConfig.Get();
            this.repo = new MockRepository(MockBehavior.Strict);
            this.cookieStorage = this.repo.Create<ICookieStorage>();
            this.getIp = this.repo.Create<IGetIp>();
            this.threadService = this.repo.Create<IThreadService>();
            this.postService = this.repo.Create<IPostService>();
            this.bannedImageLogger = this.repo.Create<IBannedImageLogger>();
            this.iIsAdmin = this.repo.Create<IIsAdmin>();

            this.thread = new ThreadModel(
                new IpHasherFactory(conf),
                new TripCodeHasher(conf),
                this.cookieStorage.Object,
                this.getIp.Object,
                this.threadService.Object,
                this.postService.Object,
                this.bannedImageLogger.Object, 
                iIsAdmin.Object)
            { PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext(), };
        }

        [Fact]
        public void AllowGet()
        {
            var threadId = Guid.NewGuid();
            var boardId = Guid.NewGuid();

            this.threadService.Setup(a => a.GetThread(threadId)).Returns(
                Task.FromResult(Option.Some(new ThreadDetailView(threadId, "subject", new BoardOverView(boardId, "random", "bee"), new List<PostOverView>() {
                    new PostOverView(Guid.NewGuid(), new DateTime(2000, 12, 25), "name", "comment", Option.None<Domain.File>(), false, "myip") }))));
            this.cookieStorage.Setup(a => a.ReadName(It.IsAny<HttpRequest>())).Returns("Matt");
            thread.OnGet(threadId, Guid.NewGuid()).Wait();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AllowPostValid()
        {
            var threadId = Guid.NewGuid();

            thread.Post = new AddPost(threadId, "Matt", "sage", "comment", null);
            this.getIp.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);
            this.cookieStorage.Setup(a => a.SetNameCookie(It.IsAny<HttpResponse>(), "Matt"));
            this.postService.Setup(a => a.Add(It.IsAny<Guid>(), threadId, It.IsAny<TripCodedName>(), "comment", true, It.IsAny<IIpHash>(), Option.None<Domain.File>())).Returns(
                Task.FromResult<OneOf<Success, Banned, ImageCountExceeded, PostCountExceeded>>(new Success()));
            this.threadService.Setup(a => a.GetThread(threadId)).Returns(Task.FromResult(Option.Some(new ThreadDetailView(threadId, "aaa", new BoardOverView(Guid.NewGuid(), "bbbb", "b"), new List<PostOverView>() ))));
            this.bannedImageLogger.Setup(a => a.Log(null, IPAddress.Loopback, It.IsAny<IIpHash>()));

            thread.OnPostAsync().Wait();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AllowPostValidWithFile()
        {
            var threadId = Guid.NewGuid();
            var file = FileMock.GetIFormFileMock(this.repo);

            thread.Post = new AddPost(threadId, "Matt", "sage", "comment", file.Object);
            this.getIp.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);
            this.cookieStorage.Setup(a => a.SetNameCookie(It.IsAny<HttpResponse>(), "Matt"));
            this.threadService.Setup(a => a.GetThread(threadId)).Returns(Task.FromResult(Option.Some(new ThreadDetailView(threadId, "aaa", new BoardOverView(Guid.NewGuid(), "bbbb", "b"), new List<PostOverView>()))));
            this.bannedImageLogger.Setup(a => a.Log(null, IPAddress.Loopback, It.IsAny<IIpHash>()));
            this.postService.Setup(a => a.Add(It.IsAny<Guid>(), threadId, It.IsAny<TripCodedName>(), "comment", true, It.IsAny<IIpHash>(), It.IsAny<Option<Domain.File>>()))
                .Returns(Task.FromResult(OneOf<Success, Banned, ImageCountExceeded, PostCountExceeded>.FromT0(new Success())));
            thread.OnPostAsync().Wait();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AllowPostInvalid()
        {
            var threadId = Guid.NewGuid();
            var boardId = Guid.NewGuid();

            thread.Post = new AddPost(threadId, "", "", "", null);
            thread.ModelState.AddModelError("file", "blah");

            this.getIp.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);
            this.threadService.Setup(a => a.GetThread(threadId)).Returns(
                Task.FromResult(Option.Some(new ThreadDetailView(threadId, "subject", new BoardOverView(boardId, "random", "bee"), new List<PostOverView>() {
                    new PostOverView(Guid.NewGuid(), new DateTime(2000, 12, 25), "name", "comment", Option.None<Domain.File>(), false, "myip") }))));
            this.bannedImageLogger.Setup(a => a.Log(null, IPAddress.Loopback, It.IsAny<IIpHash>()));

            thread.OnPostAsync().Wait();

            this.repo.VerifyAll();
        }
    }
}
