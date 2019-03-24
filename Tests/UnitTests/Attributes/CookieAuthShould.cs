using System;
using System.Collections.Generic;
using Cookies;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Optional;
using ShitForum;
using ShitForum.Attributes;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests.Attributes
{
    public class CookieAuthShould
    {
        private static HttpContext GetHttpContext(MockRepository mr)
        {
            var httpCtx = mr.Create<HttpContext>();
            httpCtx.Setup(s => s.Request).Returns(mr.Create<HttpRequest>().Object);
            return httpCtx.Object;
        }

        private readonly MockRepository mr = new MockRepository(MockBehavior.Strict);

        [Fact]
        public void NoToken()
        {
            var recapchaVerifierMock = mr.Create<ICookieStorage>();
            recapchaVerifierMock.Setup(a => a.ReadAdmin(It.IsAny<HttpRequest>())).Returns(Option.None<Guid>());

            var httpCtx = GetHttpContext(mr);
            var pageContext = new PageContext()
            {
                HttpContext = httpCtx,
                RouteData = mr.Create<RouteData>().Object,
                ActionDescriptor = mr.Create<CompiledPageActionDescriptor>().Object
            };
            var ctx = new PageHandlerExecutingContext(
                pageContext,
                new List<IFilterMetadata>(), null, new Dictionary<string, object>(), new object());
            var attr = new CookieAuthAttribute(recapchaVerifierMock.Object, new AdminSettings(MockConfig.GetAdminSettings())) as IPageFilter;

            attr.OnPageHandlerExecuting(ctx);

            ctx.Result.Should().BeOfType<ForbidResult>();
            mr.VerifyAll();
        }

        [Fact]
        public void WrongToken()
        {
            var recapchaVerifierMock = mr.Create<ICookieStorage>();
            recapchaVerifierMock.Setup(a => a.ReadAdmin(It.IsAny<HttpRequest>())).Returns(Option.Some(new Guid("FF68640c-5759-4be2-ab50-d6cd5cd6ba68")));

            var httpCtx = GetHttpContext(mr);
            var pageContext = new PageContext()
            {
                HttpContext = httpCtx,
                RouteData = mr.Create<RouteData>().Object,
                ActionDescriptor = mr.Create<CompiledPageActionDescriptor>().Object
            };
            var ctx = new PageHandlerExecutingContext(
                pageContext,
                new List<IFilterMetadata>(), null, new Dictionary<string, object>(), new object());
            var attr = new CookieAuthAttribute(recapchaVerifierMock.Object, new AdminSettings(MockConfig.GetAdminSettings())) as IPageFilter;

            attr.OnPageHandlerExecuting(ctx);
            ctx.Result.Should().BeOfType<ForbidResult>();
            mr.VerifyAll();
        }

        [Fact]
        public void Success()
        {
            var recapchaVerifierMock = mr.Create<ICookieStorage>();
            recapchaVerifierMock.Setup(a => a.ReadAdmin(It.IsAny<HttpRequest>())).Returns(Option.Some(new Guid("3c68640c-5759-4be2-ab50-d6cd5cd6ba68")));

            var httpCtx = GetHttpContext(mr);
            var pageContext = new PageContext()
            {
                HttpContext = httpCtx,
                RouteData = mr.Create<RouteData>().Object,
                ActionDescriptor = mr.Create<CompiledPageActionDescriptor>().Object
            };
            var ctx = new PageHandlerExecutingContext(
                pageContext,
                new List<IFilterMetadata>(), null, new Dictionary<string, object>(), new object());
            var attr = new CookieAuthAttribute(recapchaVerifierMock.Object, new AdminSettings(MockConfig.GetAdminSettings())) as IPageFilter;

            attr.OnPageHandlerExecuting(ctx);
            ctx.Result.Should().BeNull();
            mr.VerifyAll();
        }
    }
}
