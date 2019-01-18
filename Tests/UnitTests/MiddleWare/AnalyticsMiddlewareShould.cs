using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cookies;
using Domain;
using ExtremeIpLookup;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Optional;
using Services.Interfaces;
using ShitForum.Analytics;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests.MiddleWare
{
    public class AnalyticsMiddlewareShould
    {
        private static Mock<HttpContext> GetHttpContext(MockRepository mr)
        {
            var httpCtx = mr.Create<HttpContext>();
            var ci = mr.Create<ConnectionInfo>();
            ci.Setup(a => a.RemoteIpAddress).Returns(IPAddress.Loopback);
            httpCtx.Setup(s => s.Connection).Returns(ci.Object);
            return httpCtx;
        }

        private readonly MockRepository mr = new MockRepository(MockBehavior.Strict);

        [Fact]
        public void Fail()
        {
            var analyticsService = mr.Create<IAnalyticsService>();
            var ipLookup = mr.Create<IExtremeIpLookup>();
            ipLookup.Setup(a => a.GetIpDetailsAsync(IPAddress.Loopback)).ReturnsT(new ResultObject() {Status = "fail"});
            var logger = mr.Create<ILogger<AnalyticsMiddleware>>();
            var cs = mr.Create<ICookieStorage>();
            var attr = new AnalyticsMiddleware(_ => Task.CompletedTask, analyticsService.Object, ipLookup.Object, cs.Object, logger.Object);
            attr.InvokeAsync(GetHttpContext(mr).Object).Wait();
            mr.VerifyAll();
        }

        [Fact]
        public void Success()
        {
            var analyticsService = mr.Create<IAnalyticsService>();
            analyticsService.Setup(a => a.Add(It.IsAny<AnalyticsReport>(), CancellationToken.None)).Returns(Task.CompletedTask);
            var ipLookup = mr.Create<IExtremeIpLookup>();
            ipLookup.Setup(a => a.GetIpDetailsAsync(IPAddress.Loopback)).ReturnsT(new ResultObject() { Status = "success" });
            var logger = mr.Create<ILogger<AnalyticsMiddleware>>();
            var cs = mr.Create<ICookieStorage>();
            cs.Setup(a => a.ReadThumbPrint(It.IsAny<HttpRequest>())).Returns(Option.Some(Guid.NewGuid()));
            
            var httpContext = GetHttpContext(mr);
            httpContext.Setup(s => s.Request).Returns(mr.Create<HttpRequest>().Object);
            httpContext.Setup(s => s.Response).Returns(mr.Create<HttpResponse>().Object);
            var attr = new AnalyticsMiddleware(_ => Task.CompletedTask, analyticsService.Object, ipLookup.Object, cs.Object, logger.Object);

            attr.InvokeAsync(httpContext.Object).Wait();
            mr.VerifyAll();
        }
    }
}
