using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Subtext.Extensibility;
using Subtext.Framework;
using Subtext.Framework.Components;
using Subtext.Framework.Providers;
using Subtext.Web.SiteMap;

namespace UnitTests.Subtext.SubtextWeb
{
    [TestClass]
    public class SitemapHandlerTests
    {
        [TestMethod]
        public void ProcessRequest_WithSingleBlogPost_ProducesSitemapWithBlogPostNode()
        {
            //arrange
            var entries = new List<Entry>();
            entries.Add(new Entry(PostType.BlogPost)
            {Id = 123, DateModifiedUtc = DateTime.ParseExact("2008/01/23", "yyyy/MM/dd", CultureInfo.InvariantCulture)});
            var repository = new Mock<ObjectRepository>();
            repository.Setup(r => r.GetPostCountsByMonth()).Returns(new List<ArchiveCount>());
            repository.Setup(
                r => r.GetEntries(It.IsAny<int>(), PostType.BlogPost, It.IsAny<PostConfig>(), It.IsAny<bool>())).Returns
                (entries);
            repository.Setup(
                r => r.GetEntries(It.IsAny<int>(), PostType.Story, It.IsAny<PostConfig>(), It.IsAny<bool>())).Returns(
                new List<Entry>());
            repository.Setup(r => r.GetCategories(CategoryType.PostCollection, true)).Returns(new List<LinkCategory>());

            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.SetupSet(c => c.HttpContext.Response.ContentType, It.IsAny<string>());
            StringWriter writer = subtextContext.FakeSitemapHandlerRequest(repository);
            subtextContext.Setup(c => c.Blog).Returns(new Blog {Host = "localhost"});
            var handler = new SiteMapHttpHandler(subtextContext.Object);

            //act
            handler.ProcessRequest();

            //assert
            var doc = new XmlDocument();
            doc.LoadXml(writer.ToString());
            XmlNode entryUrlNode = doc.DocumentElement.ChildNodes[1];
            Assert.AreEqual("http://localhost/some-blogpost-with-id-of-123", entryUrlNode.ChildNodes[0].InnerText);
            Assert.AreEqual("2008-01-23", entryUrlNode.ChildNodes[1].InnerText);
        }

        [TestMethod]
        public void ProcessRequest_WithSingleArticle_ProducesSitemapWithArticleNode()
        {
            //arrange
            var entries = new List<Entry>();
            entries.Add(new Entry(PostType.Story)
            {Id = 321, DateModifiedUtc = DateTime.ParseExact("2008/01/23", "yyyy/MM/dd", CultureInfo.InvariantCulture)});
            var repository = new Mock<ObjectRepository>();
            repository.Setup(r => r.GetPostCountsByMonth()).Returns(new List<ArchiveCount>());
            repository.Setup(
                r => r.GetEntries(It.IsAny<int>(), PostType.BlogPost, It.IsAny<PostConfig>(), It.IsAny<bool>())).Returns
                (new List<Entry>());
            repository.Setup(
                r => r.GetEntries(It.IsAny<int>(), PostType.Story, It.IsAny<PostConfig>(), It.IsAny<bool>())).Returns(
                entries);
            repository.Setup(r => r.GetCategories(CategoryType.PostCollection, true)).Returns(new List<LinkCategory>());

            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.SetupSet(c => c.HttpContext.Response.ContentType, It.IsAny<string>());

            StringWriter writer = subtextContext.FakeSitemapHandlerRequest(repository);
            var handler = new SiteMapHttpHandler(subtextContext.Object);
            subtextContext.Setup(c => c.Blog).Returns(new Blog {Host = "localhost"});

            //act
            handler.ProcessRequest();

            //assert
            var doc = new XmlDocument();
            doc.LoadXml(writer.ToString());
            XmlNode entryUrlNode = doc.DocumentElement.ChildNodes[1];
            Assert.AreEqual("http://localhost/some-article-with-id-of-321", entryUrlNode.ChildNodes[0].InnerText);
            Assert.AreEqual("2008-01-23", entryUrlNode.ChildNodes[1].InnerText);
        }

        [TestMethod]
        [Ignore("Currently throws a NullReferenceException in Transformer.GetLinkFromLinkCategory"
            + "-- TODO: Determine why UrlHelper.GetVirtualPath returns null for category route")]
        public void ProcessRequest_WithSingleCategory_ProducesSitemapWithCategoryNode()
        {
            //arrange
            var categories = new List<LinkCategory>();
            categories.Add(new LinkCategory(1, "Category-1"));
            var repository = new Mock<ObjectRepository>();
            repository.Setup(r => r.GetPostCountsByMonth()).Returns(new List<ArchiveCount>());
            repository.Setup(
                r => r.GetEntries(It.IsAny<int>(), PostType.BlogPost, It.IsAny<PostConfig>(), It.IsAny<bool>())).Returns
                (new List<Entry>());
            repository.Setup(
                r => r.GetEntries(It.IsAny<int>(), PostType.Story, It.IsAny<PostConfig>(), It.IsAny<bool>())).Returns(
                new List<Entry>());
            repository.Setup(r => r.GetCategories(CategoryType.PostCollection, true)).Returns(categories);

            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.SetupSet(c => c.HttpContext.Response.ContentType, It.IsAny<string>());
            StringWriter writer = subtextContext.FakeSitemapHandlerRequest(repository);
            subtextContext.Setup(c => c.Blog).Returns(new Blog { Host = "localhost" });
            var handler = new SiteMapHttpHandler(subtextContext.Object);

            //act
            handler.ProcessRequest();

            //assert
            var doc = new XmlDocument();
            doc.LoadXml(writer.ToString());
            XmlNode entryUrlNode = doc.DocumentElement.ChildNodes[1];
            Assert.AreEqual("http://localhost/category/Category-1.aspx", entryUrlNode.ChildNodes[0].InnerText);
            Assert.AreEqual(DateTime.Now.ToString("yyyy-MM-dd"), entryUrlNode.ChildNodes[1].InnerText);
        }

        [TestMethod]
        [Ignore("Currently throws an ArgumentNullException"
            + "-- TODO: Determine why UrlHelper.MonthUrl returns null for archive route")]
        public void ProcessRequest_WithSingleArchiveMonth_ProducesSitemapWithArchiveNode()
        {
            //arrange
            var archiveCounts = new List<ArchiveCount>();
            archiveCounts.Add(new ArchiveCount()
            {
                Count = 1,
                Date = DateTime.ParseExact("2008/01/01", "yyyy/MM/dd", CultureInfo.InvariantCulture)
            });
            var repository = new Mock<ObjectRepository>();
            repository.Setup(r => r.GetPostCountsByMonth()).Returns(archiveCounts);
            repository.Setup(
                r => r.GetEntries(It.IsAny<int>(), PostType.BlogPost, It.IsAny<PostConfig>(), It.IsAny<bool>())).Returns
                (new List<Entry>());
            repository.Setup(
                r => r.GetEntries(It.IsAny<int>(), PostType.Story, It.IsAny<PostConfig>(), It.IsAny<bool>())).Returns(
                new List<Entry>());
            repository.Setup(r => r.GetCategories(CategoryType.PostCollection, true)).Returns(new List<LinkCategory>());

            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.SetupSet(c => c.HttpContext.Response.ContentType, It.IsAny<string>());
            StringWriter writer = subtextContext.FakeSitemapHandlerRequest(repository);
            subtextContext.Setup(c => c.Blog).Returns(new Blog { Host = "localhost" });
            var handler = new SiteMapHttpHandler(subtextContext.Object);

            //act
            handler.ProcessRequest();

            //assert
            var doc = new XmlDocument();
            doc.LoadXml(writer.ToString());
            XmlNode entryUrlNode = doc.DocumentElement.ChildNodes[1];
            Assert.AreEqual("http://localhost/archive/2008/01.aspx", entryUrlNode.ChildNodes[0].InnerText);
            Assert.AreEqual(DateTime.Now.ToString("yyyy-MM-dd"), entryUrlNode.ChildNodes[1].InnerText);
        }
    }
}
