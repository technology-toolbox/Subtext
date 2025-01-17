using System;
using System.Web;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Subtext.Framework.Util;

namespace UnitTests.Subtext.Framework.Util
{
    [TestClass]
    public class RequestExtensionsTests
    {
        [TestMethod]
        public void GetDateFromRequest_WithDateValuesInRouteData_ReturnsCorrectDate()
        {
            // arrange
            var routeData = new RouteData();
            routeData.Values.Add("year", "2009");
            routeData.Values.Add("month", "01");
            routeData.Values.Add("day", "23");
            var requestContext = new RequestContext(new Mock<HttpContextBase>().Object, routeData);

            // act
            DateTime theDate = requestContext.GetDateFromRequest();

            // assert
            Assert.AreEqual(new DateTime(2009, 01, 23), theDate);
        }

        [TestMethod]
        public void GetDateFromRequest_WithNoDayValueInRouteValues_ReturnsFirstDayOfMonth()
        {
            // arrange
            var routeData = new RouteData();
            routeData.Values.Add("year", "2009");
            routeData.Values.Add("month", "01");

            var requestContext = new RequestContext(new Mock<HttpContextBase>().Object, routeData);

            // act
            DateTime theDate = requestContext.GetDateFromRequest();

            // assert
            Assert.AreEqual(new DateTime(2009, 01, 01), theDate);
        }

        [TestMethod]
        public void GetSlugFromRequest_WithSlugInRouteData_ReturnsSlug()
        {
            // arrange
            var routeData = new RouteData();
            routeData.Values.Add("slug", "my-category");
            var requestContext = new RequestContext(new Mock<HttpContextBase>().Object, routeData);

            // act
            string slug = requestContext.GetSlugFromRequest();

            // assert
            Assert.AreEqual("my-category", slug);
        }

        [TestMethod]
        public void GetSlugFromRequest_WithoutSlugInRouteData_ReturnsNull()
        {
            // arrange
            var routeData = new RouteData();
            var requestContext = new RequestContext(new Mock<HttpContextBase>().Object, routeData);

            // act
            string slug = requestContext.GetSlugFromRequest();

            // assert
            Assert.IsNull(slug);
        }

        [TestMethod]
        public void GetIdFromRequest_WithIdInRouteData_ReturnsId()
        {
            // arrange
            var routeData = new RouteData();
            routeData.Values.Add("id", "123");
            var requestContext = new RequestContext(new Mock<HttpContextBase>().Object, routeData);

            // act
            int? id = requestContext.GetIdFromRequest();

            // assert
            Assert.AreEqual(123, id);
        }

        [TestMethod]
        public void GetIdFromRequest_WithoutIdInRouteData_ReturnsNull()
        {
            // arrange
            var routeData = new RouteData();
            var requestContext = new RequestContext(new Mock<HttpContextBase>().Object, routeData);

            // act
            int? id = requestContext.GetIdFromRequest();

            // assert
            Assert.IsNull(id);
        }

        [TestMethod]
        public void GetIdFromRequest_WithNonNumericIdInRouteData_ReturnsNull()
        {
            // arrange
            var routeData = new RouteData();
            routeData.Values.Add("id", "a1aoeu23");
            var requestContext = new RequestContext(new Mock<HttpContextBase>().Object, routeData);

            // act
            int? id = requestContext.GetIdFromRequest();

            // assert
            Assert.IsNull(id);
        }
    }
}