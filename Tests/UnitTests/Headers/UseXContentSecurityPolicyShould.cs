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
    public class UseXContentSecurityPolicyShould
    {
        [Fact]
        public void Fail()
        {
            var mr = new MockRepository(MockBehavior.Strict);
            var ab = mr.Create<IApplicationBuilder>();
            var httpResponse = mr.Create<HttpResponse>();
            httpResponse.Setup(a => a.Headers).Returns(new HeaderDictionary());
            var httpContext = mr.Create<HttpContext>();
            httpContext.Setup(s => s.Response).Returns(httpResponse.Object);
            ab.Setup(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Returns<Func<RequestDelegate, RequestDelegate>>(a =>
                {
                    a(_ => Task.CompletedTask)(httpContext.Object);
                    return ab.Object;
                });
            Action action = () => ab.Object.UseXContentSecurityPolicy();
            var e = action.Should().Throw<Exception>();
            e.WithMessage("Content-Security-Policy header must be set before adding IE11 compatibility");
            mr.VerifyAll();
        }

        [Fact]
        public void Success()
        {
            var mr = new MockRepository(MockBehavior.Strict);
            var ab = mr.Create<IApplicationBuilder>();
            var httpResponse = mr.Create<HttpResponse>();
            var hd = new HeaderDictionary() { { "Content-Security-Policy", "default-src: self;" } };
            httpResponse.Setup(a => a.Headers).Returns(hd);
            var httpContext = mr.Create<HttpContext>();
            httpContext.Setup(s => s.Response).Returns(httpResponse.Object);
            ab.Setup(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Returns<Func<RequestDelegate, RequestDelegate>>(a =>
                {
                    a(_ => Task.CompletedTask)(httpContext.Object);
                    return ab.Object;
                });
            ab.Object.UseXContentSecurityPolicy();
            mr.VerifyAll();
            hd.Count.Should().Be(2);
        }
    }
}
