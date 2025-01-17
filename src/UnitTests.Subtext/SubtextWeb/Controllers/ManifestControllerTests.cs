﻿using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Subtext.Framework;
using Subtext.Web.Controllers;
using UrlHelper=Subtext.Framework.Routing.BlogUrlHelper;

namespace UnitTests.Subtext.SubtextWeb.Controllers
{
    [TestClass]
    public class ManifestControllerTests
    {
        [TestMethod]
        public void Index_ReturnsContentWithContentTypeTextXml()
        {
            // arrange
            var urlHelper = new Mock<UrlHelper>();
            urlHelper.Setup(u => u.AdminUrl(It.IsAny<string>())).Returns("/");
            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.Setup(c => c.Blog).Returns(new Blog {Host = "localhost"});
            subtextContext.Setup(c => c.UrlHelper).Returns(urlHelper.Object);
            var controller = new ManifestController(subtextContext.Object);

            // act
            var manifest = controller.Index() as ContentResult;

            // assert
            Assert.AreEqual("text/xml", manifest.ContentType);
        }

        [TestMethod]
        public void Index_WithBlogNotAllowingTrackbacks_ReturnsManifestWithSupportsTrackbacksNo()
        {
            // arrange
            var urlHelper = new Mock<UrlHelper>();
            urlHelper.Setup(u => u.AdminUrl(It.IsAny<string>())).Returns("/");
            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.Setup(c => c.Blog).Returns(new Blog { Host = "localhost", TrackbacksEnabled = false });
            subtextContext.Setup(c => c.UrlHelper).Returns(urlHelper.Object);
            var controller = new ManifestController(subtextContext.Object);

            // act
            var manifest = controller.Index() as ContentResult;

            // assert
            StringAssert.Contains(manifest.Content, "<supportsTrackbacks>No</supportsTrackbacks>");
        }

        [TestMethod]
        public void Index_WithBlogAllowingTrackbacks_ReturnsManifestWithSupportsTrackbacksYes()
        {
            // arrange
            var urlHelper = new Mock<UrlHelper>();
            urlHelper.Setup(u => u.AdminUrl(It.IsAny<string>())).Returns("/");
            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.Setup(c => c.Blog).Returns(new Blog { Host = "localhost", TrackbacksEnabled = true });
            subtextContext.Setup(c => c.UrlHelper).Returns(urlHelper.Object);
            var controller = new ManifestController(subtextContext.Object);

            // act
            var manifest = controller.Index() as ContentResult;

            // assert
            StringAssert.Contains(manifest.Content, "<supportsTrackbacks>Yes</supportsTrackbacks>");
        }

        [TestMethod]
        public void Index_ReturnsManifestWithProperAdminUrls()
        {
            // arrange
            var urlHelper = new Mock<UrlHelper>();
            urlHelper.Setup(u => u.AdminUrl("")).Returns("/admin/default.aspx");
            urlHelper.Setup(u => u.AdminUrl("posts/edit.aspx")).Returns("/admin/posts/edit.aspx");
            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.Setup(c => c.Blog).Returns(new Blog { Host = "localhost" });
            subtextContext.Setup(c => c.UrlHelper).Returns(urlHelper.Object);
            var controller = new ManifestController(subtextContext.Object);

            // act
            var manifest = controller.Index() as ContentResult;

            // assert
            string expected =
                @"<adminUrl>
      <![CDATA[
        http://localhost/admin/default.aspx
    ]]>
    </adminUrl>
    <postEditingUrl>
      <![CDATA[
        http://localhost/admin/posts/edit.aspx
    ]]>
    </postEditingUrl>";

            StringAssert.Contains(manifest.Content, expected);
        }
    }
}
