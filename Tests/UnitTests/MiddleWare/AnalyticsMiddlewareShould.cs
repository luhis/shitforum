using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cookies;
using Domain;
using ExtremeIpLookup;
using Microsoft.AspNetCore.Http;
using Moq;
using Optional;
using Services;
using Services.Interfaces;
using ShitForum.Analytics;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests.MiddleWare
{
    public class AnalyticsMiddlewareShould
    {
        private static Mock<HttpContext> GetHttpContext(MockRepository mr, IServiceProvider sp)
        {
            var httpCtx = mr.Create<HttpContext>();
            httpCtx.Setup(s => s.RequestServices).Returns(sp);
            var ci = mr.Create<ConnectionInfo>();
            ci.Setup(a => a.RemoteIpAddress).Returns(IPAddress.Loopback);
            httpCtx.Setup(s => s.Connection).Returns(ci.Object);
            return httpCtx;
        }

        private readonly MockRepository mr = new MockRepository(MockBehavior.Strict);
        private readonly AnalyticsMiddleware attr = new AnalyticsMiddleware(_ => Task.CompletedTask);

        [Fact]
        public void Fail()
        {
            var analyticsService = mr.Create<IAnalyticsService>().Object;
            var ipLookup = mr.Create<IExtremeIpLookup>();
            ipLookup.Setup(a => a.GetIpDetailsAsync(IPAddress.Loopback)).ReturnsT(new ResultObject() {Status = "fail"});
            var sp = mr.Create<IServiceProvider>();
            sp.Setup(a => a.GetService(typeof(IAnalyticsService))).Returns(analyticsService);
            sp.Setup(a => a.GetService(typeof(IExtremeIpLookup))).Returns(ipLookup.Object);
            attr.InvokeAsync(GetHttpContext(mr, sp.Object).Object).Wait();
            mr.VerifyAll();
        }

        [Fact]
        public void Success()
        {
            var analyticsService = mr.Create<IAnalyticsService>();
            analyticsService.Setup(a => a.Add(It.IsAny<AnalyticsReport>(), CancellationToken.None)).Returns(Task.CompletedTask);
            var ipLookup = mr.Create<IExtremeIpLookup>();
            ipLookup.Setup(a => a.GetIpDetailsAsync(IPAddress.Loopback)).ReturnsT(new ResultObject() { Status = "success" });
            var cs = mr.Create<ICookieStorage>();
            cs.Setup(a => a.ReadThumbPrint(It.IsAny<HttpRequest>())).Returns(Option.Some(Guid.NewGuid()));

            var sp = mr.Create<IServiceProvider>();
            sp.Setup(a => a.GetService(typeof(IAnalyticsService))).Returns(analyticsService.Object);
            sp.Setup(a => a.GetService(typeof(IExtremeIpLookup))).Returns(ipLookup.Object);
            sp.Setup(a => a.GetService(typeof(ICookieStorage))).Returns(cs.Object);
            var httpContext = GetHttpContext(mr, sp.Object);
            httpContext.Setup(s => s.Request).Returns(mr.Create<HttpRequest>().Object);
            httpContext.Setup(s => s.Response).Returns(mr.Create<HttpResponse>().Object);

            attr.InvokeAsync(httpContext.Object).Wait();
            mr.VerifyAll();
        }
    }
}
