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
        private static HttpContext GetHttpContext(MockRepository mr)
        {
            var httpReq = mr.Create<HttpRequest>();
            httpReq.Setup(a => a.Method).Returns("POST");

            var httpCtx = mr.Create<HttpContext>();
            httpCtx.Setup(s => s.Request).Returns(httpReq.Object);
            return httpCtx.Object;
        }

        private readonly MockRepository mr = new MockRepository(MockBehavior.Strict);

        [Fact]
        public void Pass()
        {
            var recapchaMock = mr.Create<IGetCaptchaValue>();
            recapchaMock.Setup(a => a.Get(It.IsAny<HttpRequest>())).Returns("capchaValue");
            var recapchaVerifierMock = mr.Create<IRecaptchaVerifier>();
            recapchaVerifierMock.Setup(a => a.IsValid("capchaValue", IPAddress.Loopback)).ReturnsT(true);
            var ipMock = mr.Create<IGetIp>();
            ipMock.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);

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
            var attr = new RecaptchaAttribute(recapchaVerifierMock.Object, recapchaMock.Object, ipMock.Object) as IAsyncPageFilter;

            attr.OnPageHandlerExecutionAsync(ctx, () => Task.FromResult(new PageHandlerExecutedContext(
                pageContext, 
                new List<IFilterMetadata>(), null, new object()))).Wait();
            ctx.ModelState.IsValid.Should().BeTrue();
            mr.VerifyAll();
        }

        [Fact]
        public void Fail()
        {
            var recapchaMock = mr.Create<IGetCaptchaValue>();
            recapchaMock.Setup(a => a.Get(It.IsAny<HttpRequest>())).Returns("capchaValue");
            var recapchaVerifierMock = mr.Create<IRecaptchaVerifier>();
            recapchaVerifierMock.Setup(a => a.IsValid("capchaValue", IPAddress.Loopback)).ReturnsT(false);
            var ipMock = mr.Create<IGetIp>();
            ipMock.Setup(a => a.GetIp(It.IsAny<HttpRequest>())).Returns(IPAddress.Loopback);

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
            var attr = new RecaptchaAttribute(recapchaVerifierMock.Object, recapchaMock.Object, ipMock.Object) as IAsyncPageFilter;

            attr.OnPageHandlerExecutionAsync(ctx, () => Task.FromResult(new PageHandlerExecutedContext(
                pageContext,
                new List<IFilterMetadata>(), null, new object()))).Wait();
            ctx.ModelState.IsValid.Should().BeFalse();
            mr.VerifyAll();
        }
    }
}
