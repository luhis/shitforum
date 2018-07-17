using System.IO;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UnitTests.Tooling
{
    public static class FileMock
    {
        public static Mock<IFormFile> GetIFormFileMock(MockRepository mr)
        {
            var m = mr.Create<IFormFile>();
            m.Setup(a => a.FileName).Returns("jezza.jpg");
            var f = System.IO.File.ReadAllBytes("../../../images/jezza.jpg");
            m.Setup(a => a.OpenReadStream()).Returns(new MemoryStream(f));
            return m;
        }
    }
}