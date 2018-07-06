using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Moq;
using Optional;
using ShitForum;
using ShitForum.Attributes;
using ShitForum.Cookies;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.Attributes
{
    public class CookieAuthShould
    {
        [Fact]
        public void Run()
        {
            var cfb = new ConfigurationBuilder();
            cfb.AddInMemoryCollection(new[] { KeyValuePair.Create("Gods:0", Guid.NewGuid().ToString()) });
            var mr = new MockRepository(MockBehavior.Strict);
            var attr = new CookieAuthAttribute() as IPageFilter;
            var httpCtx = mr.Create<HttpContext>();
            var httpReq = mr.Create<HttpRequest>();
            httpCtx.Setup(s => s.Request).Returns(httpReq.Object);
            var sp = mr.Create<IServiceProvider>();
            sp.Setup(a => a.GetService(typeof(AdminSettings))).Returns(new AdminSettings(cfb.Build()));
            var recapchaVerifierMock = mr.Create<ICookieStorage>();
            recapchaVerifierMock.Setup(a => a.ReadAdmin(It.IsAny<HttpRequest>())).Returns(Option.None<Guid>());
            sp.Setup(a => a.GetService(typeof(ICookieStorage))).Returns(recapchaVerifierMock.Object);
      
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
            attr.OnPageHandlerExecuting(ctx);
            mr.VerifyAll();
        }
    }
}
