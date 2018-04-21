using Moq;
using ShitForum.Pages;
using System;
using System.Threading.Tasks;
using Xunit;
using Services;
using Optional;
using Services.Dtos;

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

            this.threadService.Setup(a => a.GetOrderedCatalogThreads(boardId, 100, 0)).Returns(Task.FromResult(Option.Some(
                new CatalogThreadOverViewSet(new Domain.Board(boardId, "b", "bbb"), new CatalogThreadOverView[] {
                    new CatalogThreadOverView(Guid.NewGuid(), "subject", new Domain.Board(boardId, "b", "board"), 
                    new PostOverView(Guid.NewGuid(), new DateTime(2000, 12, 25), "name", "comment", Option.None<Domain.File>(), false, "127.0.0.1")) })
                )));
            thread.OnGet(boardId).Wait();

            this.repo.VerifyAll();
        }
    }
}
