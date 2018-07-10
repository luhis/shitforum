using System;
using System.Collections.Generic;
using System.Threading;
using Domain;
using Domain.Repositories;
using Moq;
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
        public void Test()
        {
            this.bir.Setup(a => a.GetAll(ct))
                .ReturnsT(new List<BannedImage>() {new BannedImage(Guid.NewGuid(), "", "")});
            var r = this.fs.GetAllBannedImages(ct).Result;
        }
    }
}
