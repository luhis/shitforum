using System;
using Cookies;
using FluentAssertions;
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
        public void ReturnFalse()
        {
            var hr = repo.Create<HttpRequest>();
            var context = GetHttpContext(this.repo, hr.Object);
            cookie.Setup(a => a.ReadAdmin(It.IsAny<HttpRequest>())).Returns(Option.Some(Guid.NewGuid()));
            var r = this.isAdmin.IsAdmin(context);
            r.Should().BeFalse();
            this.repo.VerifyAll();
        }

        [Fact]
        public void ReturnTrue()
        {
            var hr = repo.Create<HttpRequest>();
            var context = GetHttpContext(this.repo, hr.Object);
            cookie.Setup(a => a.ReadAdmin(It.IsAny<HttpRequest>())).Returns(Option.Some(new Guid("3c68640c-5759-4be2-ab50-d6cd5cd6ba68")));
            var r = this.isAdmin.IsAdmin(context);
            r.Should().BeTrue();
            this.repo.VerifyAll();
        }
    }
}
