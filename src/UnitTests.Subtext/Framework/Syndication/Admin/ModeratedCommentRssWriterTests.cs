using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Subtext.Extensibility;
using Subtext.Framework;
using Subtext.Framework.Components;
using Subtext.Framework.Routing;
using Subtext.Framework.Syndication;
using Subtext.Framework.Syndication.Admin;
using UnitTests.Subtext.Framework.Util;

namespace UnitTests.Subtext.Framework.Syndication.Admin
{
    [TestClass]
    public class ModeratedCommentRssWriterTests : SyndicationTestBase
    {
        /// <summary>
        /// Tests that a valid feed is produced even if a post has no comments.
        /// </summary>
        [DatabaseIntegrationTestMethod]
        public void CommentRssWriterProducesValidEmptyFeed()
        {
            UnitTestHelper.SetHttpContextWithBlogRequest("localhost", "blog");

            var blogInfo = new Blog();
            blogInfo.Host = "localhost";
            blogInfo.Subfolder = "blog";
            blogInfo.Email = "Subtext@example.com";
            blogInfo.RFC3229DeltaEncodingEnabled = true;
            blogInfo.Title = "My Blog Rulz";
            blogInfo.TimeZoneId = TimeZonesTest.PacificTimeZoneId;

            HttpContext.Current.Items.Add("BlogInfo-", blogInfo);

            var entry = new Entry(PostType.None);
            entry.AllowComments = true;
            entry.Title = "Comments requiring your approval.";
            entry.Body = "The following items are waiting approval.";
            entry.PostType = PostType.None;

            var urlHelper = new Mock<BlogUrlHelper>();
            urlHelper.Setup(url => url.ImageUrl(It.IsAny<string>())).Returns("/images/RSS2Image.gif");
            urlHelper.Setup(url => url.GetVirtualPath(It.IsAny<string>(), It.IsAny<object>())).Returns(
                "/blog/Admin/Feedback.aspx?status=2");
            urlHelper.Setup(url => url.EntryUrl(It.IsAny<Entry>())).Returns("/blog/Admin/Feedback.aspx?status=2");
            urlHelper.Setup(url => url.AdminUrl(It.IsAny<string>(), It.IsAny<object>())).Returns(
                "/blog/Admin/Feedback.aspx?status=2");
            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.Setup(c => c.Blog).Returns(blogInfo);
            subtextContext.Setup(c => c.UrlHelper).Returns(urlHelper.Object);
            var writer = new ModeratedCommentRssWriter(new StringWriter(), new List<FeedbackItem>(), entry,
                                                       subtextContext.Object);

            string expected = @"<rss version=""2.0"" "
                              + @"xmlns:dc=""http://purl.org/dc/elements/1.1/"" "
                              + @"xmlns:trackback=""http://madskills.com/public/xml/rss/module/trackback/"" "
                              + @"xmlns:wfw=""http://wellformedweb.org/CommentAPI/"" "
                              + @"xmlns:slash=""http://purl.org/rss/1.0/modules/slash/"" "
                              + @"xmlns:copyright=""http://blogs.law.harvard.edu/tech/rss"" "
                              + @"xmlns:image=""http://purl.org/rss/1.0/modules/image/"">" + Environment.NewLine
                              + indent() + @"<channel>" + Environment.NewLine
                              + indent(2) + @"<title>Comments requiring your approval.</title>" + Environment.NewLine
                              + indent(2) + @"<link>http://localhost/blog/Admin/Feedback.aspx?status=2</link>" +
                              Environment.NewLine
                              + indent(2) + @"<description>The following items are waiting approval.</description>" +
                              Environment.NewLine
                              + indent(2) + @"<language>en-US</language>" + Environment.NewLine
                              + indent(2) + @"<copyright>Subtext Weblog</copyright>" + Environment.NewLine
                              + indent(2) + @"<generator>{0}</generator>" + Environment.NewLine
                              + indent(2) + @"<image>" + Environment.NewLine
                              + indent(3) + @"<title>Comments requiring your approval.</title>" + Environment.NewLine
                              + indent(3) + @"<url>http://localhost/images/RSS2Image.gif</url>" + Environment.NewLine
                              + indent(3) + @"<link>http://localhost/blog/Admin/Feedback.aspx?status=2</link>" +
                              Environment.NewLine
                              + indent(3) + @"<width>77</width>" + Environment.NewLine
                              + indent(3) + @"<height>60</height>" + Environment.NewLine
                              + indent(2) + @"</image>" + Environment.NewLine
                              + indent(1) + @"</channel>" + Environment.NewLine
                              + @"</rss>";

            expected = string.Format(expected, VersionInfo.VersionDisplayText);

            Assert.AreEqual(expected, writer.Xml);
        }

        /// <summary>
        /// Tests that a valid feed is produced even if a post has no comments.
        /// </summary>
        [DatabaseIntegrationTestMethod]
        public void Xml_WithOneCommentNeedingModeration_ProducesValidRSSFeedWithTheOneComment()
        {
            // Arrange
            UnitTestHelper.SetHttpContextWithBlogRequest("localhost", "", "Subtext.Web");

            var blogInfo = new Blog();
            blogInfo.Host = "localhost";
            blogInfo.Email = "Subtext@example.com";
            blogInfo.RFC3229DeltaEncodingEnabled = true;
            blogInfo.Title = "My Blog Rulz";
            blogInfo.TimeZoneId = TimeZonesTest.PacificTimeZoneId;

            HttpContext.Current.Items.Add("BlogInfo-", blogInfo);

            var rootEntry = new Entry(PostType.None);
            rootEntry.AllowComments = true;
            rootEntry.Title = "Comments requiring your approval.";
            rootEntry.Body = "The following items are waiting approval.";
            rootEntry.PostType = PostType.None;

            Entry entry = UnitTestHelper.CreateEntryInstanceForSyndication(blogInfo, "haacked", "title of the post",
                                                                           "Body of the post.");
            entry.EntryName = "titleofthepost";
            entry.DateCreatedUtc =
                entry.DatePublishedUtc =
                entry.DateModifiedUtc = DateTime.ParseExact("2006/02/01 08:00", "yyyy/MM/dd hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
            entry.Id = 1001;

            var comment = new FeedbackItem(FeedbackType.Comment);
            comment.Id = 1002;
            comment.DateCreatedUtc =
                comment.DateModifiedUtc = DateTime.ParseExact("2006/02/02 07:00", "yyyy/MM/dd hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
            comment.Title = "re: titleofthepost";
            comment.ParentEntryName = entry.EntryName;
            comment.ParentDateCreatedUtc = entry.DateCreatedUtc;
            comment.Body = "<strong>I rule!</strong>";
            comment.Author = "Jane Schmane";
            comment.Email = "jane@example.com";
            comment.EntryId = entry.Id;
            comment.Status = FeedbackStatusFlag.NeedsModeration;

            var comments = new List<FeedbackItem>();
            comments.Add(comment);

            var urlHelper = new Mock<BlogUrlHelper>();
            urlHelper.Setup(url => url.EntryUrl(It.IsAny<Entry>())).Returns(
                "/Subtext.Web/archive/2006/02/01/titleofthepost.aspx");
            urlHelper.Setup(url => url.FeedbackUrl(It.IsAny<FeedbackItem>())).Returns(
                "/Subtext.Web/archive/2006/02/01/titleofthepost.aspx#1002");
            urlHelper.Setup(url => url.ImageUrl(It.IsAny<string>())).Returns("/Subtext.Web/images/RSS2Image.gif");
            urlHelper.Setup(url => url.AdminUrl(It.IsAny<string>(), It.IsAny<object>())).Returns(
                "/Subtext.Web/Admin/Feedback.aspx?status=2");
            var context = new Mock<ISubtextContext>();
            context.Setup(c => c.UrlHelper).Returns(urlHelper.Object);
            context.Setup(c => c.Blog).Returns(blogInfo);

            var writer = new ModeratedCommentRssWriter(new StringWriter(), comments, rootEntry, context.Object);

            string expected = @"<rss version=""2.0"" "
                              + @"xmlns:dc=""http://purl.org/dc/elements/1.1/"" "
                              + @"xmlns:trackback=""http://madskills.com/public/xml/rss/module/trackback/"" "
                              + @"xmlns:wfw=""http://wellformedweb.org/CommentAPI/"" "
                              + @"xmlns:slash=""http://purl.org/rss/1.0/modules/slash/"" "
                              + @"xmlns:copyright=""http://blogs.law.harvard.edu/tech/rss"" "
                              + @"xmlns:image=""http://purl.org/rss/1.0/modules/image/"">" + Environment.NewLine
                              + indent() + @"<channel>" + Environment.NewLine
                              + indent(2) + @"<title>Comments requiring your approval.</title>" + Environment.NewLine
                              + indent(2) + @"<link>http://localhost/Subtext.Web/Admin/Feedback.aspx?status=2</link>" +
                              Environment.NewLine
                              + indent(2) + @"<description>The following items are waiting approval.</description>" +
                              Environment.NewLine
                              + indent(2) + @"<language>en-US</language>" + Environment.NewLine
                              + indent(2) + @"<copyright>Subtext Weblog</copyright>" + Environment.NewLine
                              + indent(2) + @"<generator>{0}</generator>" + Environment.NewLine
                              + indent(2) + @"<image>" + Environment.NewLine
                              + indent(3) + @"<title>Comments requiring your approval.</title>" + Environment.NewLine
                              + indent(3) + @"<url>http://localhost/Subtext.Web/images/RSS2Image.gif</url>" +
                              Environment.NewLine
                              + indent(3) +
                              @"<link>http://localhost/Subtext.Web/archive/2006/02/01/titleofthepost.aspx</link>" +
                              Environment.NewLine
                              + indent(3) + @"<width>77</width>" + Environment.NewLine
                              + indent(3) + @"<height>60</height>" + Environment.NewLine
                              + indent(2) + @"</image>" + Environment.NewLine
                              + indent(2) + @"<item>" + Environment.NewLine
                              + indent(3) + @"<title>re: titleofthepost</title>" + Environment.NewLine
                              + indent(3) +
                              @"<link>http://localhost/Subtext.Web/archive/2006/02/01/titleofthepost.aspx#1002</link>" +
                              Environment.NewLine
                              + indent(3) + @"<description>&lt;strong&gt;I rule!&lt;/strong&gt;</description>" +
                              Environment.NewLine
                              + indent(3) + @"<dc:creator>Jane Schmane</dc:creator>" + Environment.NewLine
                              + indent(3) +
                              @"<guid>http://localhost/Subtext.Web/archive/2006/02/01/titleofthepost.aspx#1002</guid>" +
                              Environment.NewLine
                              + indent(3) + @"<pubDate>Thu, 02 Feb 2006 07:00:00 GMT</pubDate>" + Environment.NewLine
                              + indent(2) + @"</item>" + Environment.NewLine
                              + indent() + @"</channel>" + Environment.NewLine
                              + @"</rss>";

            expected = string.Format(expected, VersionInfo.VersionDisplayText);

            // Act
            string rss = writer.Xml;

            // Assert
            UnitTestHelper.AssertStringsEqualCharacterByCharacter(expected, rss);
            //Assert.AreEqual(expected, writer.Xml);
        }

        [TestMethod]
        public void Ctor_WithNullEntryCollection_ThrowsArgumentNullException()
        {
            UnitTestHelper.AssertThrowsArgumentNullException(() =>
                new CommentRssWriter(new StringWriter(), null, new Entry(PostType.BlogPost),
                                 new Mock<ISubtextContext>().Object)
                );

        }

        [TestMethod]
        public void Ctor_WithNullEntry_ThrowsArgumentNullException()
        {
            UnitTestHelper.AssertThrowsArgumentNullException(() =>
                new CommentRssWriter(new StringWriter(), new List<FeedbackItem>(), null, new Mock<ISubtextContext>().Object)
            );
        }
    }
}