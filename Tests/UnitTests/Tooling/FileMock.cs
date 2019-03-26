using System.Reflection;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UnitTests.Tooling
{
    public static class FileMock
    {
        public static Mock<IFormFile> GetIFormFileMock(MockRepository mr)
        {
            var m = mr.Create<IFormFile>();
            var fileName = "jezza.jpg";
            var myAssembly = Assembly.GetExecutingAssembly();
            var myStream = myAssembly.GetManifestResourceStream($"UnitTests.Images.{fileName}");

            m.Setup(a => a.FileName).Returns(fileName);
            m.Setup(a => a.OpenReadStream()).Returns(myStream);
            return m;
        }
    }
}
