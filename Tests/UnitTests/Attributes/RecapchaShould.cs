using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using ReCaptchaCore;
using ShitForum.Attributes;
using ShitForum.GetIp;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests.Attributes
{
    public class RecapchaShould
    {
        private static HttpContext GetHttpContext(MockRepository mr, IServiceProvider sp)
        {
            var httpReq = mr.Create<HttpRequest>();
            httpReq.Setup(a => a.Method).Returns("POST");

            var httpCtx = mr.Create<HttpContext>();
            httpCtx.Setup(s => s.Request).Returns(httpReq.Object);
            httpCtx.Setup(s => s.RequestServices).Returns(sp);
            return httpCtx.Object;
        }

        private readonly MockRepository mr = new MockRepository(MockBehavior.Strict);
        private readonly IAsyncPageFilter attr = new RecaptchaAttribute();

        [Fact]
        public void Pass()
        {
            var sp = mr.Create<IServiceProvider>();
            var recapchaMock = mr.Create<IGetCaptchaValue>();
            recapchaMock.Setup(a => a.Get(It.IsAny<HttpRequest>())).Returns("capchaValue");
            sp.Setup(a => a.GetService(typeof(IGetCaptchaValue))).Returns(recapchaMock.Object);
            var recapchaVerifierMock = mr.Create<IRecaptchaVerifier>();
            recapchaVerifierMock.Setup(a => a.IsValid("capchaValue", IPAddress.Loopback)).ReturnsT(true);
            sp.Setup(a => a.GetService(typeof(IRecaptchaVerifier))).Returns(recapchaVerifierMock.Object);
            var ipMock = mr.Create<IGetIp>();
            ipMock.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);
            sp.Setup(a => a.GetService(typeof(IGetIp))).Returns(ipMock.Object);

            var httpCtx = GetHttpContext(mr, sp.Object);

            var pageContext = new PageContext()
            {
                HttpContext = httpCtx,
                RouteData = mr.Create<RouteData>().Object,
                ActionDescriptor = mr.Create<CompiledPageActionDescriptor>().Object
            };
            var ctx = new PageHandlerExecutingContext(
                pageContext, 
                new List<IFilterMetadata>(), null, new Dictionary<string, object>(), new object());
            attr.OnPageHandlerExecutionAsync(ctx, () => Task.FromResult(new PageHandlerExecutedContext(
                pageContext, 
                new List<IFilterMetadata>(), null, new object()))).Wait();
            ctx.ModelState.IsValid.Should().BeTrue();
            mr.VerifyAll();
        }

        [Fact]
        public void Fail()
        {
            var sp = mr.Create<IServiceProvider>();
            var recapchaMock = mr.Create<IGetCaptchaValue>();
            recapchaMock.Setup(a => a.Get(It.IsAny<HttpRequest>())).Returns("capchaValue");
            sp.Setup(a => a.GetService(typeof(IGetCaptchaValue))).Returns(recapchaMock.Object);
            var recapchaVerifierMock = mr.Create<IRecaptchaVerifier>();
            recapchaVerifierMock.Setup(a => a.IsValid("capchaValue", IPAddress.Loopback)).ReturnsT(false);
            sp.Setup(a => a.GetService(typeof(IRecaptchaVerifier))).Returns(recapchaVerifierMock.Object);
            var ipMock = mr.Create<IGetIp>();
            ipMock.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);
            sp.Setup(a => a.GetService(typeof(IGetIp))).Returns(ipMock.Object);

            var httpCtx = GetHttpContext(mr, sp.Object);

            var pageContext = new PageContext()
            {
                HttpContext = httpCtx,
                RouteData = mr.Create<RouteData>().Object,
                ActionDescriptor = mr.Create<CompiledPageActionDescriptor>().Object
            };
            var ctx = new PageHandlerExecutingContext(
                pageContext,
                new List<IFilterMetadata>(), null, new Dictionary<string, object>(), new object());
            attr.OnPageHandlerExecutionAsync(ctx, () => Task.FromResult(new PageHandlerExecutedContext(
                pageContext,
                new List<IFilterMetadata>(), null, new object()))).Wait();
            ctx.ModelState.IsValid.Should().BeFalse();
            mr.VerifyAll();
        }
    }
}
