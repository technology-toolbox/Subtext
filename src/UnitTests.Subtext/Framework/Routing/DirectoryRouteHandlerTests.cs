using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Subtext.Framework.Routing;

namespace UnitTests.Subtext.Framework.Routing
{
    [TestClass]
    public class DirectoryRouteHandlerTests
    {
        [TestMethod]
        public void RequestContext_WithNonDirectoryRoute_CausesInvalidOperationException()
        {
            //arrange
            var httpContext = new Mock<HttpContextBase>();
            var routeData = new RouteData
                                {
                                    Route = new Route("url",
                                                      new DirectoryRouteHandler(new Mock<ISubtextPageBuilder>().Object,
                                                                                new Mock<IDependencyResolver>().Object))
                                };
            var requestContext = new RequestContext(httpContext.Object, routeData);
            IRouteHandler routeHandler = new DirectoryRouteHandler(new Mock<ISubtextPageBuilder>().Object,
                                                                   new Mock<IDependencyResolver>().Object);

            //act, assert
            UnitTestHelper.AssertThrows<InvalidOperationException>(() => routeHandler.GetHttpHandler(requestContext));
        }

        [TestMethod]
        public void RequestWithoutSubfolder_ForDirectory_GetsHandlerInPhysicalDirectory()
        {
            //arrange
            string virtualPath = string.Empty;
            var routeData = new RouteData
                                {
                                    Route = new DirectoryRoute("admin", new Mock<IDependencyResolver>().Object)
                                };
            ;
            routeData.Values.Add("pathinfo", "foo.aspx");
            var pageBuilder = new Mock<ISubtextPageBuilder>();
            var httpHandler = new Mock<IHttpHandler>();
            pageBuilder.Setup(b => b.CreateInstanceFromVirtualPath(It.IsAny<string>(), It.IsAny<Type>())).Returns(
                httpHandler.Object).Callback<string, Type>((vpath, type) => virtualPath = vpath);
            IRouteHandler routeHandler = new DirectoryRouteHandler(pageBuilder.Object, new Mock<IDependencyResolver>().Object);
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/admin/foo.aspx");
            var requestContext = new RequestContext(httpContext.Object, routeData);

            //act
            IHttpHandler handler = routeHandler.GetHttpHandler(requestContext);

            //assert
            Assert.AreEqual("~/aspx/admin/foo.aspx", virtualPath);
        }

        [TestMethod]
        public void DirectoryRouteHandler_InheritsRouteHandlerBase()
        {
            Assert.IsTrue(typeof(RouteHandlerBase).IsAssignableFrom(typeof(DirectoryRouteHandler)));
        }

        [TestMethod]
        public void RequestWithoutSubfolder_ForAshxFileInDirectory_GetsHandlerInPhysicalDirectory()
        {
            //arrange
            string virtualPath = string.Empty;
            var routeData = new RouteData
                                {
                                    Route = new DirectoryRoute("admin", new Mock<IDependencyResolver>().Object)
                                };
            ;
            routeData.Values.Add("pathinfo", "foo.ashx");
            var pageBuilder = new Mock<ISubtextPageBuilder>();
            var httpHandler = new Mock<IHttpHandler>();
            pageBuilder.Setup(b => b.CreateInstanceFromVirtualPath(It.IsAny<string>(), It.IsAny<Type>())).Returns(
                httpHandler.Object).Callback<string, Type>((vpath, type) => virtualPath = vpath);
            IRouteHandler routeHandler = new DirectoryRouteHandler(pageBuilder.Object, new Mock<IDependencyResolver>().Object);
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/admin/foo.ashx");
            var requestContext = new RequestContext(httpContext.Object, routeData);

            //act
            IHttpHandler handler = routeHandler.GetHttpHandler(requestContext);

            //assert
            Assert.AreEqual("~/aspx/admin/foo.ashx", virtualPath);
        }

        //TODO: Simplify this test.
        [TestMethod]
        public void RequestWithoutSubfolder_ForDirectoryWithoutFile_AppendsDefaultFileToVirtualPath()
        {
            //arrange
            string virtualPath = string.Empty;
            var routeData = new RouteData();
            routeData.Route = new DirectoryRoute("admin", new Mock<IDependencyResolver>().Object);
            routeData.Values.Add("pathinfo", "posts");
            var pageBuilder = new Mock<ISubtextPageBuilder>();
            var httpHandler = new Mock<IHttpHandler>();
            pageBuilder.Setup(b => b.CreateInstanceFromVirtualPath(It.IsAny<string>(), It.IsAny<Type>())).Returns(
                httpHandler.Object).Callback<string, Type>((vpath, type) => virtualPath = vpath);
            IRouteHandler routeHandler = new DirectoryRouteHandler(pageBuilder.Object, new Mock<IDependencyResolver>().Object);
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/admin/posts/");
            var requestContext = new RequestContext(httpContext.Object, routeData);

            //act
            routeHandler.GetHttpHandler(requestContext);

            //assert
            Assert.AreEqual("~/aspx/admin/posts/Default.aspx", virtualPath);
        }

        [TestMethod]
        public void RequestWithoutSubfolder_ForDirectoryWithoutFileAndWithouEndingSlash_AppendsDefaultFileToVirtualPath()
        {
            //arrange
            string virtualPath = string.Empty;
            var routeData = new RouteData
                                {
                                    Route = new DirectoryRoute("admin", new Mock<IDependencyResolver>().Object)
                                };
            ;
            routeData.Values.Add("pathinfo", "posts");
            var pageBuilder = new Mock<ISubtextPageBuilder>();
            var httpHandler = new Mock<IHttpHandler>();
            pageBuilder.Setup(b => b.CreateInstanceFromVirtualPath(It.IsAny<string>(), It.IsAny<Type>())).Returns(
                httpHandler.Object).Callback<string, Type>((vpath, type) => virtualPath = vpath);
            IRouteHandler routeHandler = new DirectoryRouteHandler(pageBuilder.Object, new Mock<IDependencyResolver>().Object);
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/admin/posts");
            var requestContext = new RequestContext(httpContext.Object, routeData);

            //act
            routeHandler.GetHttpHandler(requestContext);

            //assert
            Assert.AreEqual("~/aspx/admin/posts/Default.aspx", virtualPath);
        }

        [TestMethod]
        public void RequestWithSubfolder_ForDirectory_GetsHandlerInPhysicalDirectory()
        {
            //arrange
            string virtualPath = string.Empty;
            var routeData = new RouteData
                                {
                                    Route = new DirectoryRoute("admin", new Mock<IDependencyResolver>().Object)
                                };
            routeData.Values.Add("subfolder", "blogsubfolder");
            routeData.Values.Add("pathinfo", "foo.aspx");
            var pageBuilder = new Mock<ISubtextPageBuilder>();
            var httpHandler = new Mock<IHttpHandler>();
            pageBuilder.Setup(b => b.CreateInstanceFromVirtualPath(It.IsAny<string>(), It.IsAny<Type>())).Returns(
                httpHandler.Object).Callback<string, Type>((vpath, type) => virtualPath = vpath);
            IRouteHandler routeHandler = new DirectoryRouteHandler(pageBuilder.Object, new Mock<IDependencyResolver>().Object);
            var httpContext = new Mock<HttpContextBase>();
            httpContext.FakeRequest("~/blogsubfolder/admin/foo.aspx", "blogsubfolder");
            var requestContext = new RequestContext(httpContext.Object, routeData);

            //act
            IHttpHandler handler = routeHandler.GetHttpHandler(requestContext);

            //assert
            Assert.AreEqual("~/aspx/admin/foo.aspx", virtualPath);
        }
    }
}