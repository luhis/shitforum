using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using ReCaptchaCore;
using ShitForum.Attributes;
using ShitForum.GetIp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests.Attributes
{
    public class RecapchaShould
    {
        [Fact]
        public void Run()
        {
            var mr = new MockRepository(MockBehavior.Strict);
            var attr = new RecaptchaAttribute() as IAsyncPageFilter;
            var httpCtx = mr.Create<HttpContext>();
            var httpReq = mr.Create<HttpRequest>();
            httpReq.Setup(a => a.Method).Returns("POST");
            httpCtx.Setup(s => s.Request).Returns(httpReq.Object);
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
            httpCtx.Setup(s => s.RequestServices).Returns(sp.Object);
            var pageContext = new PageContext()
            {
                HttpContext = httpCtx.Object,
                RouteData = mr.Create<RouteData>().Object,
                ActionDescriptor = mr.Create<CompiledPageActionDescriptor>().Object
            };
            var ctx = new PageHandlerExecutingContext(
                pageContext, 
                new List<IFilterMetadata>(), null, new Dictionary<string, object>(), new object());
            attr.OnPageHandlerExecutionAsync(ctx, () => Task.FromResult<PageHandlerExecutedContext>(new PageHandlerExecutedContext(
                pageContext, 
                new List<IFilterMetadata>(), null, new object()))).Wait();
            mr.VerifyAll();
        }
    }
}
