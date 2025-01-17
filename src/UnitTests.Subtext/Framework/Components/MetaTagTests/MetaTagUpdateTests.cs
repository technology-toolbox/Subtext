#region Disclaimer/Info

///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext@googlegroups.com 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Subtext.Framework.Components;
using Subtext.Framework.Data;
using Subtext.Framework.Text;

namespace UnitTests.Subtext.Framework.Components.MetaTagTests
{
    [TestClass]
    public class MetatTagUpdateTests
    {
        [DatabaseIntegrationTestMethod]
        [DataRow("Steven Harman", "author", null)]
        [DataRow("no-cache", null, "cache-control")]
        public void CanUpdateMetaTag(string content, string name, string httpequiv)
        {
            var blog = UnitTestHelper.CreateBlogAndSetupContext();
            var repository = new DatabaseObjectProvider();
            MetaTag tag = UnitTestHelper.BuildMetaTag(content, name, httpequiv, blog.Id, null, DateTime.UtcNow);
            repository.Create(tag);

            string randomStr = UnitTestHelper.GenerateUniqueString().Left(20);
            tag.Content = content + randomStr;

            if (!string.IsNullOrEmpty(name))
            {
                tag.Name = name + randomStr;
            }

            if (!string.IsNullOrEmpty(httpequiv))
            {
                tag.HttpEquiv = httpequiv + randomStr;
            }

            Assert.IsTrue(repository.Update(tag));

            MetaTag updTag = repository.GetMetaTagsForBlog(blog, 0, 100)[0];

            ValidateMetaTags(tag, updTag);
        }

        [DatabaseIntegrationTestMethod]
        public void CanRemoveNameAndAddHttpEquiv()
        {
            var blog = UnitTestHelper.CreateBlogAndSetupContext();
            var repository = new DatabaseObjectProvider();
            MetaTag tag = UnitTestHelper.BuildMetaTag("Nothing to see here.", "description", null, blog.Id, null,
                                                      DateTime.UtcNow);
            repository.Create(tag);

            tag.HttpEquiv = "cache-control";
            tag.Name = null;
            tag.Content = "no-cache";

            repository.Update(tag);

            ValidateMetaTags(tag, repository.GetMetaTagsForBlog(blog, 0, 100)[0]);
        }

        [DatabaseIntegrationTestMethod]
        public void CanRemoveHttpEquivAndAddName()
        {
            var blog = UnitTestHelper.CreateBlogAndSetupContext();
            var repository = new DatabaseObjectProvider();
            MetaTag tag = UnitTestHelper.BuildMetaTag("Still nothing to see here.", null, "expires", blog.Id, null, DateTime.UtcNow);
            repository.Create(tag);

            tag.HttpEquiv = null;
            tag.Name = "author";
            tag.Content = "Steve-o-rino!";

            repository.Update(tag);

            ValidateMetaTags(tag, repository.GetMetaTagsForBlog(blog, 0, 100)[0]);
        }

        [TestMethod]
        public void Update_WithInvalidMetaTag_ThrowsArgumentException()
        {
            // arrange
            var metaTag = new MetaTag(null);
            var repository = new DatabaseObjectProvider();

            // act, assert
            Assert.IsFalse(metaTag.IsValid);
            UnitTestHelper.AssertThrows<ArgumentException>(() => repository.Update(metaTag));
        }

        [TestMethod]
        public void Update_WithNullMetaTag_ThrowsArgumentNullException()
        {
            var repository = new DatabaseObjectProvider();
            UnitTestHelper.AssertThrowsArgumentNullException(() => repository.Update((MetaTag)null));
        }

        private static void ValidateMetaTags(MetaTag expected, MetaTag result)
        {
            Assert.AreEqual(expected.Content, result.Content, "Content didn't get updated.");
            Assert.AreEqual(expected.Name, result.Name, "Name attribute didn't get updated.");
            Assert.AreEqual(expected.HttpEquiv, result.HttpEquiv, "Http-Equiv attribute didn't get updated");
        }
    }
}