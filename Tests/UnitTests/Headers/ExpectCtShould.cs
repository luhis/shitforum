using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;
using ShitForum;
using Xunit;

namespace UnitTests.Headers
{
    public class ExpectCtShould
    {
        [Fact]
        public void ExcludeWhenNotHttps()
        {
            var mr = new MockRepository(MockBehavior.Strict);
            var ab = mr.Create<IApplicationBuilder>();
            var httpRequest = mr.Create<HttpRequest>();
            httpRequest.Setup(a => a.IsHttps).Returns(false);
            var httpContext = mr.Create<HttpContext>();
            httpContext.Setup(s => s.Request).Returns(httpRequest.Object);
            ab.Setup(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Returns<Func<RequestDelegate, RequestDelegate>>(a =>
                {
                    a(_ => Task.CompletedTask)(httpContext.Object);
                    return ab.Object;
                });
            ab.Object.UseExpectCt();
            mr.VerifyAll();
        }

        [Fact]
        public void IncludeWhenHttps()
        {
            var mr = new MockRepository(MockBehavior.Strict);
            var ab = mr.Create<IApplicationBuilder>();
            var httpRequest = mr.Create<HttpRequest>();
            httpRequest.Setup(a => a.IsHttps).Returns(true);
            var httpResponse = mr.Create<HttpResponse>();
            var hd = new HeaderDictionary();
            httpResponse.Setup(a => a.Headers).Returns(hd);
            var httpContext = mr.Create<HttpContext>();
            httpContext.Setup(s => s.Request).Returns(httpRequest.Object);
            httpContext.Setup(s => s.Response).Returns(httpResponse.Object);
            ab.Setup(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Returns<Func<RequestDelegate, RequestDelegate>>(a =>
                {
                    a(_ => Task.CompletedTask)(httpContext.Object);
                    return ab.Object;
                });
            ab.Object.UseExpectCt();
            mr.VerifyAll();
            hd.Count.Should().Be(1);
        }
    }
}
