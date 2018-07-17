using System;
using Cookies;
using Microsoft.AspNetCore.Http;
using Moq;
using Optional;
using ShitForum;
using ShitForum.IsAdmin;
using Tests.UnitTests.Tooling;
using Xunit;

namespace UnitTests.Services
{
    public class IsAdminShould
    {
        private static HttpContext GetHttpContext(MockRepository mr, HttpRequest httpRequest)
        {
            var httpCtx = mr.Create<HttpContext>();
            httpCtx.Setup(s => s.Request).Returns(httpRequest);
            return httpCtx.Object;
        }

        private readonly MockRepository repo;
        private readonly IIsAdmin isAdmin;
        private readonly Mock<ICookieStorage> cookie;

        public IsAdminShould()
        {
            this.repo = new MockRepository(MockBehavior.Strict);
            var settings = new AdminSettings(MockConfig.Get());
            this.cookie = repo.Create<ICookieStorage>();
            this.isAdmin = new IsAdmin(cookie.Object, settings);
        }

        [Fact]
        public void Test()
        {
            var hr = repo.Create<HttpRequest>();
            var context = GetHttpContext(this.repo, hr.Object);
            cookie.Setup(a => a.ReadAdmin(It.IsAny<HttpRequest>())).Returns(Option.Some<Guid>(Guid.NewGuid()));
            this.isAdmin.IsAdmin(context);
            this.repo.VerifyAll();
        }
    }
}
