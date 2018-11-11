using Domain;
using Microsoft.AspNetCore.Http;
using Moq;
using Optional;
using Services;
using Hashers;
using ShitForum.Models;
using ShitForum.Pages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cookies;
using Domain.IpHash;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Services.Dtos;
using Xunit;
using Services.Results;
using OneOf;
using Services.Interfaces;
using ShitForum.BannedImageLogger;
using ShitForum.GetIp;
using ShitForum.Mappers;
using Tests.UnitTests.Tooling;
using UnitTests.Tooling;
using File = Domain.File;
using ThumbNailer;

namespace UnitTests.Pages
{
    public class BoardShould
    {
        private readonly MockRepository repo;
        private readonly Mock<ICookieStorage> cookieStorage;
        private readonly BoardModel board;
        private readonly Mock<IGetIp> getIp;
        private readonly Mock<IThreadService> threadService;
        private readonly Mock<IPostService> postService;
        private readonly Mock<IBannedImageLogger> bannedImageLogger;
        private readonly IUploadMapper uploadMapper;
        private readonly CancellationToken ct = CancellationToken.None;

        public BoardShould()
        {
            this.repo = new MockRepository(MockBehavior.Strict);
            this.cookieStorage = this.repo.Create<ICookieStorage>();
            this.getIp = this.repo.Create<IGetIp>();
            this.threadService = this.repo.Create<IThreadService>();
            this.postService = this.repo.Create<IPostService>();
            this.bannedImageLogger = this.repo.Create<IBannedImageLogger>();
            this.uploadMapper = new UploadMapper(new Thumbnailer(MockConfig.GetThumbNailerSettings()));

            this.board = new BoardModel(
                new IpHasherFactory(MockConfig.GetHasherSettings()),
                new TripCodeHasher(MockConfig.GetTripCodeHasherSettings()),
                cookieStorage.Object,
                this.getIp.Object,
                this.threadService.Object,
                this.postService.Object,
                this.bannedImageLogger.Object,
                this.uploadMapper)
            { PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext(), };
        }

        [Fact]
        public void AllowGet()
        {
            var boardId = Guid.NewGuid();

            this.threadService.Setup(a => a.GetOrderedThreads("bee", Option.None<string>(), 100, 1, ct)).ReturnsT(
                Option.Some(
                    new ThreadOverViewSet(
                        new Board(boardId, "random", "bee"),
                        new List<ThreadOverView>()
                        {
                            new ThreadOverView(Guid.NewGuid(), "subject",
                                new PostOverView(Guid.NewGuid(), new DateTime(2000, 12, 25), "name", "IP", "comment",
                                    Option.None<Domain.File>()), new List<PostOverView> { }, new ThreadOverViewStats(1, 1))
                        }, new PageData(1, 11))));

            this.cookieStorage.Setup(a => a.ReadName(It.IsAny<HttpRequest>())).Returns("Matt");
            board.OnGet("bee", null, ct).Wait();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AllowPostValid()
        {
            var boardId = Guid.NewGuid();

            board.Thread = new AddThread(boardId, "Matt", "sage", "subject", "comment", null);
            this.cookieStorage.Setup(a => a.SetNameCookie(It.IsAny<HttpResponse>(), "Matt"));
            this.getIp.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);
            this.postService.Setup(a => a.AddThread(It.IsAny<Guid>(), It.IsAny<Guid>(), boardId, "subject", It.IsAny<TripCodedName>(), "comment", true, It.IsAny<IIpHash>(), Option.None<File>(), ct)).Returns(Task.FromResult<OneOf<Success, Banned>>(new Success()));
            this.threadService.Setup(a => a.GetOrderedThreads("bee", Option.None<string>(), 100, 1, ct))
                .ReturnsT(Option.Some(new ThreadOverViewSet(new Board(boardId, "bbbb", "b"), new List<ThreadOverView>(), new PageData(1, 11))));
            this.bannedImageLogger.Setup(a => a.Log(null, IPAddress.Loopback, It.IsAny<IIpHash>()));

            board.OnPostAsync("bee", null, ct).Wait();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AllowPostValidWithFile()
        {
            var boardId = Guid.NewGuid();
            var file = FileMock.GetIFormFileMock(this.repo);

            board.Thread = new AddThread(boardId, "Matt", "sage", "subject", "comment", file.Object);
            this.getIp.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);
            this.cookieStorage.Setup(a => a.SetNameCookie(It.IsAny<HttpResponse>(), "Matt"));
            this.threadService.Setup(a => a.GetOrderedThreads("bee", Option.None<string>(), 100, 1, ct))
                .ReturnsT(Option.Some(new ThreadOverViewSet(new Board(boardId, "bbbb", "b"), new List<ThreadOverView>(), new PageData(1, 11))));
            this.bannedImageLogger.Setup(a => a.Log(null, IPAddress.Loopback, It.IsAny<IIpHash>()));
            this.postService.Setup(a => 
                a.AddThread(It.IsAny<Guid>(), It.IsAny<Guid>(), boardId, "subject", It.IsAny<TripCodedName>(), "comment", true, It.IsAny<IIpHash>(), It.IsAny<Option<File>>(), ct)).Returns(Task.FromResult<OneOf<Success, Banned>>(new Success()));

            board.OnPostAsync("bee", null, ct).Wait();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AllowPostInvalid()
        {
            var boardId = Guid.NewGuid();

            board.Thread = new AddThread(boardId, "", "", "", "", null);
            board.ModelState.AddModelError("file", "blah");

            this.getIp.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);
            this.threadService.Setup(a => a.GetOrderedThreads("bee", Option.None<string>(), 100, 1, ct)).ReturnsT(
                Option.Some(
                    new ThreadOverViewSet(
                        new Board(boardId, "random", "bee"),
                        new List<ThreadOverView>()
                        {
                            new ThreadOverView(Guid.NewGuid(), "subject",
                                new PostOverView(Guid.NewGuid(), new DateTime(2000, 12, 25), "name", "IP", "comment",
                                    Option.None<File>()), new List<PostOverView> { }, new ThreadOverViewStats(1, 1))
                        }, new PageData(1, 11))));
            this.bannedImageLogger.Setup(a =>
                a.Log(It.IsAny<ModelStateEntry>(), IPAddress.Loopback, It.IsAny<IIpHash>()));

            board.OnPostAsync("bee", null, ct).Wait();

            this.repo.VerifyAll();
        }
    }
}
