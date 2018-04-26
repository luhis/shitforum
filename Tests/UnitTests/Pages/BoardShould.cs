﻿using Domain;
using Microsoft.AspNetCore.Http;
using Moq;
using Optional;
using Services;
using ShitForum.Hasher;
using ShitForum.Models;
using ShitForum.Pages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Domain.IpHash;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Services.Dtos;
using Xunit;
using Services.Results;
using OneOf;
using ShitForum;
using ShitForum.BannedImageLogger;
using ShitForum.Cookies;
using ShitForum.GetIp;

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

        public BoardShould()
        {
            var conf = MockConfig.Get();
            this.repo = new MockRepository(MockBehavior.Strict);
            this.cookieStorage = this.repo.Create<ICookieStorage>();
            this.getIp = this.repo.Create<IGetIp>();
            this.threadService = this.repo.Create<IThreadService>();
            this.postService = this.repo.Create<IPostService>();
            this.bannedImageLogger = this.repo.Create<IBannedImageLogger>();

            this.board = new BoardModel(
                new IpHasherFactory(conf),
                new TripCodeHasher(conf),
                cookieStorage.Object,
                this.getIp.Object,
                this.threadService.Object,
                this.postService.Object,
                this.bannedImageLogger.Object)
            { PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext(), };
        }

        [Fact]
        public void AllowGet()
        {
            var boardId = Guid.NewGuid();

            this.threadService.Setup(a => a.GetOrderedThreads(boardId, Option.None<string>(), 100, 0)).Returns(
                Task.FromResult(Option.Some(
                    new ThreadOverViewSet(
                        new Board(boardId, "random", "bee"),
                        new List<ThreadOverView>() { new ThreadOverView(Guid.NewGuid(), "subject",
                        new PostOverView(Guid.NewGuid(), new DateTime(2000, 12, 25), "name", "comment", Option.None<Domain.File>(), false, "127.0.0.1"), new List<PostOverView>() { } , 1, 1) }))));

            this.cookieStorage.Setup(a => a.ReadName(It.IsAny<HttpRequest>())).Returns("Matt");
            board.OnGet(boardId, null).Wait();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AllowPostValid()
        {
            var boardId = Guid.NewGuid();

            board.Thread = new AddThread(boardId, "Matt", "sage", "subject", "comment", null);
            this.cookieStorage.Setup(a => a.SetNameCookie(It.IsAny<HttpResponse>(), "Matt"));
            this.getIp.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);
            this.postService.Setup(a => a.AddThread(It.IsAny<Guid>(), It.IsAny<Guid>(), boardId, "subject", It.IsAny<TripCodedName>(), "comment", true, It.IsAny<IIpHash>(), Option.None<File>())).Returns(Task.FromResult<OneOf<Success, Banned>>(new Success()));
            this.threadService.Setup(a => a.GetOrderedThreads(boardId, Option.None<string>(), 100, 0))
                .Returns(Task.FromResult(Option.Some(new ThreadOverViewSet(new Board(boardId, "bbbb", "b"), new List<ThreadOverView>()))));
            this.bannedImageLogger.Setup(a => a.Log(null, IPAddress.Loopback, It.IsAny<IIpHash>()));

            board.OnPostAsync(null).Wait();

            this.repo.VerifyAll();
        }

        [Fact(Skip = "need to look at this")]
        public void AllowPostValidWithFIle()
        {
            var boardId = Guid.NewGuid();
            var file = this.repo.Create<IFormFile>(MockBehavior.Loose).Object;

            board.Thread = new AddThread(boardId, "Matt", "sage", "subject", "comment", file);
            this.getIp.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);
            this.cookieStorage.Setup(a => a.SetNameCookie(It.IsAny<HttpResponse>(), "Matt"));
            board.OnPostAsync(null).Wait();

            this.repo.VerifyAll();
        }

        [Fact]
        public void AllowPostInvalid()
        {
            var boardId = Guid.NewGuid();

            board.Thread = new AddThread(boardId, "", "", "", "", null);
            board.ModelState.AddModelError("file", "blah");

            this.getIp.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);
            this.threadService.Setup(a => a.GetOrderedThreads(boardId, Option.None<string>(), 100, 0)).Returns(
               Task.FromResult(Option.Some(
                   new ThreadOverViewSet(
                       new Board(boardId, "random", "bee"),
                       new List<ThreadOverView>() { new ThreadOverView(Guid.NewGuid(), "subject",
                       new PostOverView(Guid.NewGuid(), new DateTime(2000, 12, 25), "name", "comment", Option.None<File>(), false, "127.0.0.1"), new List<PostOverView>() { }, 1, 1) }))));
            this.bannedImageLogger.Setup(a => a.Log(It.IsAny<ModelStateEntry>(), IPAddress.Loopback, It.IsAny<IIpHash>()));

            board.OnPostAsync(null).Wait();

            this.repo.VerifyAll();
        }
    }
}
