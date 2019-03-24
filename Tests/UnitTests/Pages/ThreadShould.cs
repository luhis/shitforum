using Moq;
using Hashers;
using ShitForum.Pages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;
using ShitForum.Models;
using System.Net;
using System.Threading;
using Cookies;
using Optional;
using Services;
using Services.Dtos;
using Services.Results;
using OneOf;
using Domain.IpHash;
using Services.Interfaces;
using ShitForum.BannedImageLogger;
using ShitForum.GetIp;
using ShitForum.IsAdmin;
using UnitTests.Tooling;
using ShitForum.Mappers;
using ThumbNailer;

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
        private readonly IUploadMapper uploadMapper;
        private readonly CancellationToken ct = CancellationToken.None;

        public ThreadShould()
        {
            var conf = MockConfig.GetAdminSettings();
            this.repo = new MockRepository(MockBehavior.Strict);
            this.cookieStorage = this.repo.Create<ICookieStorage>();
            this.getIp = this.repo.Create<IGetIp>();
            this.threadService = this.repo.Create<IThreadService>();
            this.postService = this.repo.Create<IPostService>();
            this.bannedImageLogger = this.repo.Create<IBannedImageLogger>();
            this.iIsAdmin = this.repo.Create<IIsAdmin>();
            this.uploadMapper = new UploadMapper(new ThumbNailer.ThumbNailer(MockConfig.GetThumbNailerSettings()));

            this.thread = new ThreadModel(
                new IpHasherFactory(MockConfig.GetHasherSettings()),
                new TripCodeHasher(MockConfig.GetTripCodeHasherSettings()),
                this.cookieStorage.Object,
                this.getIp.Object,
                this.threadService.Object,
                this.postService.Object,
                this.bannedImageLogger.Object,
                this.iIsAdmin.Object,
                this.uploadMapper)
            { PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext(), };
        }

        [Fact]
        public void AllowGet()
        {
            var threadId = Guid.NewGuid();
            var boardId = Guid.NewGuid();
            this.iIsAdmin.Setup(a => a.IsAdmin(It.IsAny<HttpContext>())).Returns(false);
            this.threadService.Setup(a => a.GetThread(threadId, 100, ct)).ReturnsT(
                Option.Some(new ThreadDetailView(threadId, "subject", new ThreadStats(10, 1, 10, 1), new BoardOverView(boardId, "random", "bee"), new List<PostOverView>() {
                    new PostOverView(Guid.NewGuid(), new DateTime(2000, 12, 25), "name", "IP", "comment", Option.None<Domain.File>()) })));
            this.cookieStorage.Setup(a => a.ReadName(It.IsAny<HttpRequest>())).Returns("Matt");
            thread.OnGet("bee", threadId, Guid.NewGuid(), CancellationToken.None).Wait();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AllowPostValid()
        {
            var threadId = Guid.NewGuid();

            thread.Post = new AddPost(threadId, "Matt", "sage", "comment", null);
            this.getIp.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);
            this.cookieStorage.Setup(a => a.SetNameCookie(It.IsAny<HttpResponse>(), "Matt"));
            this.postService.Setup(a => a.Add(It.IsAny<Guid>(), threadId, It.IsAny<TripCodedName>(), "comment", true, It.IsAny<IIpHash>(), Option.None<Domain.File>(), ct)).Returns(
                Task.FromResult<OneOf<Success, Banned, ImageCountExceeded, PostCountExceeded>>(new Success()));
            this.threadService.Setup(a => a.GetThread(threadId, 100, ct)).ReturnsT(Option.Some(new ThreadDetailView(threadId, "aaa", new ThreadStats(10, 1, 10, 1), new BoardOverView(Guid.NewGuid(), "bbbb", "b"), new List<PostOverView>() )));
            this.bannedImageLogger.Setup(a => a.Log(null, IPAddress.Loopback, It.IsAny<IIpHash>()));

            thread.OnPostAsync(ct).Wait();

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
            this.threadService.Setup(a => a.GetThread(threadId, 100, ct)).ReturnsT(Option.Some(new ThreadDetailView(threadId, "aaa", new ThreadStats(10, 1, 10, 1), new BoardOverView(Guid.NewGuid(), "bbbb", "b"), new List<PostOverView>())));
            this.bannedImageLogger.Setup(a => a.Log(null, IPAddress.Loopback, It.IsAny<IIpHash>()));
            this.postService.Setup(a => a.Add(It.IsAny<Guid>(), threadId, It.IsAny<TripCodedName>(), "comment", true, It.IsAny<IIpHash>(), It.IsAny<Option<Domain.File>>(), ct))
                .ReturnsT((OneOf<Success, Banned, ImageCountExceeded, PostCountExceeded>)new Success());
            thread.OnPostAsync(ct).Wait();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AllowPostInvalid()
        {
            var threadId = Guid.NewGuid();
            var boardId = Guid.NewGuid();

            thread.Post = new AddPost(threadId, "", "", "", null);
            thread.ModelState.AddModelError("file", "blah");

            this.iIsAdmin.Setup(a => a.IsAdmin(It.IsAny<HttpContext>())).Returns(false);
            this.getIp.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);
            this.threadService.Setup(a => a.GetThread(threadId, 100, ct)).Returns(
                Task.FromResult(Option.Some(new ThreadDetailView(threadId, "subject", new ThreadStats(10, 1, 10, 1), new BoardOverView(boardId, "random", "bee"), new List<PostOverView>() {
                    new PostOverView(Guid.NewGuid(), new DateTime(2000, 12, 25), "name", "IP", "comment", Option.None<Domain.File>()) }))));
            this.bannedImageLogger.Setup(a => a.Log(null, IPAddress.Loopback, It.IsAny<IIpHash>()));

            thread.OnPostAsync(ct).Wait();

            this.repo.VerifyAll();
        }
    }
}
