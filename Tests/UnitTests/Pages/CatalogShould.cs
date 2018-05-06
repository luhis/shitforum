using Moq;
using ShitForum.Pages;
using System;
using Xunit;
using Services;
using Optional;
using Services.Dtos;
using UnitTests.Tooling;

namespace UnitTests.Pages
{
    public class CatalogShould
    {
        private readonly MockRepository repo;
        private readonly Mock<IThreadService> threadService;
        private readonly CatalogModel thread;

        public CatalogShould()
        {
            this.repo = new MockRepository(MockBehavior.Strict);
            this.threadService = this.repo.Create<IThreadService>();

            this.thread = new CatalogModel(
                this.threadService.Object)
            { PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext(), };
        }

        [Fact]
        public void AllowGet()
        {
            var boardId = Guid.NewGuid();

            this.threadService.Setup(a => a.GetOrderedCatalogThreads("bee")).ReturnsT(Option.Some(
                new CatalogThreadOverViewSet(new Domain.Board(boardId, "b", "bbb"), new CatalogThreadOverView[] {
                    new CatalogThreadOverView(Guid.NewGuid(), "subject", new Domain.Board(boardId, "b", "board"), 
                        new PostOverView(Guid.NewGuid(), new DateTime(2000, 12, 25), "name", "comment", Option.None<Domain.File>())) })
            ));
            thread.OnGet("bee").Wait();

            this.repo.VerifyAll();
        }
    }
}
