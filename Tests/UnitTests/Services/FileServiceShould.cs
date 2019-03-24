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
using Services.Services;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests.Services
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
            this.fr = this.repo.Create<IFileRepository>();
            this.bir = this.repo.Create<IBannedImageRepository>();
            this.fs =  new FileService(this.fr.Object, this.bir.Object);
        }

        [Fact]
        public void GetAll()
        {
            this.bir.Setup(a => a.GetAll(this.ct))
                .ReturnsT(new List<BannedImage>() {new BannedImage(Guid.NewGuid(), "", "")});
            var r = this.fs.GetAllBannedImages(this.ct).Result;
            this.repo.VerifyAll();
        }

        [Fact]
        public void GetById()
        {
            var postId = Guid.NewGuid();
            this.fr.Setup(a => a.GetPostFile(postId, this.ct)).ReturnsT(new Option<File>());
            var r = this.fs.GetPostFile(postId, this.ct).Result;
            this.repo.VerifyAll();
        }

        [Fact]
        public void BanImage()
        {
            var ih = new ImageHash("aaaa");
            this.bir.Setup(a => a.Ban(ih, "reason", this.ct)).Returns(Task.CompletedTask);
            this.fs.BanImage(ih, "reason", this.ct).Wait();
            this.repo.VerifyAll();
        }
    }
}
