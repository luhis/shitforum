using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Moq;
using Optional;
using Services;
using Services.Interfaces;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests
{
    public class FileServiceShould
    {
        private readonly MockRepository repo;
        private readonly IFileService fs;
        private readonly CancellationToken ct = CancellationToken.None;
        private readonly Mock<IFileRepository> fr;
        private readonly Mock<IBannedImageRepository> bir;

        public FileServiceShould()
        {
            this.repo = new MockRepository(MockBehavior.Strict);
            this.fr = repo.Create<IFileRepository>();
            this.bir = repo.Create<IBannedImageRepository>();
            this.fs =  new FileService(this.fr.Object, this.bir.Object);
        }

        [Fact]
        public void GetAll()
        {
            this.bir.Setup(a => a.GetAll(ct))
                .ReturnsT(new List<BannedImage>() {new BannedImage(Guid.NewGuid(), "", "")});
            var r = this.fs.GetAllBannedImages(ct).Result;
            repo.VerifyAll();
        }

        [Fact]
        public void GetById()
        {
            var postId = Guid.NewGuid();
            this.fr.Setup(a => a.GetPostFile(postId, ct)).ReturnsT(new Option<File>());
            var r = this.fs.GetPostFile(postId, ct).Result;
            repo.VerifyAll();
        }

        [Fact]
        public void BanImage()
        {
            var ih = new ImageHash("aaaa");
            this.bir.Setup(a => a.Ban(ih, "reason", ct)).Returns(Task.CompletedTask);
            this.fs.BanImage(ih, "reason", ct).Wait();
            repo.VerifyAll();
        }
    }
}
