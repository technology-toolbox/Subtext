using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using STRouting = Subtext.Framework.Routing;

namespace UnitTests.Subtext.Framework.Routing
{
    [TestClass]
    public class RootRouteTests
    {
        [TestMethod]
        public void GetRouteDataWithRequestForAppRoot_WhenAggregationEnabled_MatchesAndReturnsAggDefault()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/", string.Empty /* subfolder */, "~/");
            var route = new STRouting.RootRoute(true, new Mock<IDependencyResolver>().Object);

            //act
            RouteData routeData = route.GetRouteData(httpContext.Object);

            //assert
            var routeHandler = routeData.RouteHandler as STRouting.PageRouteHandler;
            Assert.AreEqual("~/aspx/AggDefault.aspx", routeHandler.VirtualPath);
            Assert.AreSame(route, routeData.Route);
            Assert.IsFalse(routeData.DataTokens.ContainsKey(STRouting.PageRoute.ControlNamesKey));
        }

        [TestMethod]
        public void GetRouteDataWithRequestForAppRoot_WhenAggregationDisabled_MatchesAndReturnsDtp()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/", string.Empty /* subfolder */, "~/");
            var route = new STRouting.RootRoute(false, new Mock<IDependencyResolver>().Object);

            //act
            RouteData routeData = route.GetRouteData(httpContext.Object);

            //assert
            var routeHandler = routeData.RouteHandler as STRouting.PageRouteHandler;
            Assert.AreEqual("~/aspx/Dtp.aspx", routeHandler.VirtualPath);
            Assert.AreSame(route, routeData.Route);
            Assert.IsTrue(routeData.DataTokens.ContainsKey(STRouting.PageRoute.ControlNamesKey));
        }

        [TestMethod]
        public void GetRouteDataWithRequestForSubfolder_WhenAggregationEnabled_MatchesRequestAndReturnsDtp()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/subfolder", "subfolder" /* subfolder */, "~/");
            var route = new STRouting.RootRoute(true, new Mock<IDependencyResolver>().Object);

            //act
            RouteData routeData = route.GetRouteData(httpContext.Object);

            //assert
            var routeHandler = routeData.RouteHandler as STRouting.PageRouteHandler;
            Assert.AreEqual("~/aspx/Dtp.aspx", routeHandler.VirtualPath);
            Assert.AreSame(route, routeData.Route);
        }

        [TestMethod]
        public void GetRouteDataWithRequestForSubfolder_WhenAggregationDisabled_MatchesRequestAndReturnsDtp()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/subfolder", "subfolder" /* subfolder */, "~/");
            var route = new STRouting.RootRoute(false, new Mock<IDependencyResolver>().Object);

            //act
            RouteData routeData = route.GetRouteData(httpContext.Object);

            //assert
            var routeHandler = routeData.RouteHandler as STRouting.PageRouteHandler;
            Assert.AreEqual("~/aspx/Dtp.aspx", routeHandler.VirtualPath);
            Assert.AreSame(route, routeData.Route);
        }

        [TestMethod]
        public void GetRouteDataWithRequestWithSubfolder_WhenAggregationEnabledAndBlogDoesNotHaveSubfolder_DoesNotMatch()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/foo", string.Empty /* subfolder */, "~/");
            var route = new STRouting.RootRoute(true, new Mock<IDependencyResolver>().Object);

            //act
            RouteData routeData = route.GetRouteData(httpContext.Object);

            //assert
            Assert.IsNull(routeData);
        }

        [TestMethod]
        public void GetRouteDataWithRequestWithSubfolder_WhenAggregationDisabledAndBlogDoesNotHaveSubfolder_DoesNotMatch
            ()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/foo", string.Empty /* subfolder */, "~/");
            var route = new STRouting.RootRoute(false, new Mock<IDependencyResolver>().Object);

            //act
            RouteData routeData = route.GetRouteData(httpContext.Object);

            //assert
            Assert.IsNull(routeData);
        }

        [TestMethod]
        public void
            GetRouteDataWithRequestWithSubfolder_WhenAggregationEnabledAndSubfolderDoesNotMatchBlogSubfolder_DoesNotMatch
            ()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/foo", "bar" /* subfolder */, "~/");
            var route = new STRouting.RootRoute(true, new Mock<IDependencyResolver>().Object);

            //act
            RouteData routeData = route.GetRouteData(httpContext.Object);

            //assert
            Assert.IsNull(routeData);
        }

        [TestMethod]
        public void
            GetRouteDataWithRequestWithSubfolder_WhenAggregationDisabledAndSubfolderDoesNotMatchBlogSubfolder_DoesNotMatch
            ()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/foo", "bar" /* subfolder */, "~/");
            var route = new STRouting.RootRoute(false, new Mock<IDependencyResolver>().Object);

            //act
            RouteData routeData = route.GetRouteData(httpContext.Object);

            //assert
            Assert.IsNull(routeData);
        }

        [TestMethod]
        public void GetRouteDataWithRequestForDefault_WhenAggregationEnabled_MatchesAndReturnsAggDefault()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/Default.aspx", string.Empty /* subfolder */, "~/");
            var route = new STRouting.RootRoute(true, new Mock<IDependencyResolver>().Object);

            //act
            RouteData routeData = route.GetRouteData(httpContext.Object);

            //assert
            var routeHandler = routeData.RouteHandler as STRouting.PageRouteHandler;
            Assert.AreEqual("~/aspx/AggDefault.aspx", routeHandler.VirtualPath);
            Assert.AreSame(route, routeData.Route);
        }

        [TestMethod]
        public void GetRouteDataWithRequestForDefault_WhenAggregationDisabled_MatchesAndReturnsDtp()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/Default.aspx", string.Empty /* subfolder */, "~/");
            var route = new STRouting.RootRoute(false, new Mock<IDependencyResolver>().Object);

            //act
            RouteData routeData = route.GetRouteData(httpContext.Object);

            //assert
            var routeHandler = routeData.RouteHandler as STRouting.PageRouteHandler;
            Assert.AreEqual("~/aspx/Dtp.aspx", routeHandler.VirtualPath);
            Assert.AreSame(route, routeData.Route);
        }

        [TestMethod]
        public void GetRouteDataWithRequestForDefaultInSubfolder_WhenAggregationEnabled_MatchesRequestAndReturnsDtp()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/subfolder/default.aspx", "subfolder" /* subfolder */, "~/");
            var route = new STRouting.RootRoute(true, new Mock<IDependencyResolver>().Object);

            //act
            RouteData routeData = route.GetRouteData(httpContext.Object);

            //assert
            var routeHandler = routeData.RouteHandler as STRouting.PageRouteHandler;
            Assert.AreEqual("~/aspx/Dtp.aspx", routeHandler.VirtualPath);
            Assert.AreSame(route, routeData.Route);
        }

        [TestMethod]
        public void GetRouteDataWithRequestForDefaultInSubfolder_WhenAggregationDisabled_MatchesRequestAndReturnsDtp()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/subfolder/default.aspx", "subfolder" /* subfolder */, "~/");
            var route = new STRouting.RootRoute(false, new Mock<IDependencyResolver>().Object);

            //act
            RouteData routeData = route.GetRouteData(httpContext.Object);

            //assert
            var routeHandler = routeData.RouteHandler as STRouting.PageRouteHandler;
            Assert.AreEqual("~/aspx/Dtp.aspx", routeHandler.VirtualPath);
            Assert.AreSame(route, routeData.Route);
        }

        [TestMethod]
        public void GetVirtualPath_WhenAggregationEnabledAndNoSubfolderInRouteData_ReturnsRoot()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/default.aspx", string.Empty /* subfolder */, "~/");
            var routeData = new RouteData();
            var requestContext = new RequestContext(httpContext.Object, routeData);
            var route = new STRouting.RootRoute(true, new Mock<IDependencyResolver>().Object);
            var routeValues = new RouteValueDictionary();

            //act
            VirtualPathData virtualPathInfo = route.GetVirtualPath(requestContext, routeValues);

            //assert
            Assert.AreEqual(string.Empty, virtualPathInfo.VirtualPath);
        }

        [TestMethod]
        public void GetVirtualPath_WhenAggregationEnabledWithSubfolderInRouteData_ReturnsSubfolder()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/subfolder/default.aspx", "subfolder" /* subfolder */, "~/");
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "subfolder");
            var requestContext = new RequestContext(httpContext.Object, routeData);
            var route = new STRouting.RootRoute(true, new Mock<IDependencyResolver>().Object);
            var routeValues = new RouteValueDictionary();

            //act
            VirtualPathData virtualPathInfo = route.GetVirtualPath(requestContext, routeValues);

            //assert
            Assert.AreEqual("subfolder", virtualPathInfo.VirtualPath);
        }

        [TestMethod]
        public void GetVirtualPath_WhenAggregationEnabledWithSubfolderInRouteValues_ReturnsSubfolder()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/subfolder/default.aspx", "subfolder" /* subfolder */, "~/");
            var routeData = new RouteData();
            var requestContext = new RequestContext(httpContext.Object, routeData);
            var route = new STRouting.RootRoute(true, new Mock<IDependencyResolver>().Object);
            var routeValues = new RouteValueDictionary(new { subfolder = "subfolder" });

            //act
            VirtualPathData virtualPathInfo = route.GetVirtualPath(requestContext, routeValues);

            //assert
            Assert.AreEqual("subfolder", virtualPathInfo.VirtualPath);
        }

        [TestMethod]
        public void GetVirtualPath_WhenSupplyingRouteValues_AppendsValuesToQueryString()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/subfolder/default.aspx", string.Empty /* subfolder */, "~/");
            var routeData = new RouteData();
            var requestContext = new RequestContext(httpContext.Object, routeData);
            var route = new STRouting.RootRoute(true, new Mock<IDependencyResolver>().Object);
            var routeValues = new RouteValueDictionary(new { foo = "bar" });

            //act
            VirtualPathData virtualPathInfo = route.GetVirtualPath(requestContext, routeValues);

            //assert
            Assert.AreEqual(virtualPathInfo.VirtualPath, "?foo=bar");
        }
    }
}