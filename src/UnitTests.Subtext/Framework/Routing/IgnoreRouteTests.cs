using System.Web;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Subtext.Framework.Routing;

namespace UnitTests.Subtext.Framework.Routing
{
    [TestClass]
    public class IgnoreRouteTests
    {
        [TestMethod]
        public void GetVirtualPath_WithIgnoreRoute_AlwaysReturnsNull()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            var requestContext = new RequestContext(httpContext.Object, new RouteData());
            var route = new IgnoreRoute("{*catchall}");

            //act
            VirtualPathData virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary());

            //assert
            Assert.IsNull(virtualPath);
        }
    }
}