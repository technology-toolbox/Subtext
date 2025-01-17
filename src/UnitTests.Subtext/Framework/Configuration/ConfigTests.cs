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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Subtext.Framework;
using Subtext.Framework.Configuration;
using Subtext.Framework.Data;

namespace UnitTests.Subtext.Framework.Configuration
{
    /// <summary>
    /// These are unit tests specifically for the Config class.
    /// </summary>
    [TestClass]
    public class ConfigTests
    {
        string hostName;

        /// <summary>
        /// Make sure we can correctly find a blog based on it's HostName and
        /// subfolder when the system has multiple blogs with the same Host.
        /// </summary>
        [DatabaseIntegrationTestMethod]
        public void GetBlogInfoFindsBlogWithUniqueHostAndSubfolder()
        {
            var repository = new DatabaseObjectProvider();
            string subfolder1 = UnitTestHelper.GenerateUniqueString();
            string subfolder2 = UnitTestHelper.GenerateUniqueString();
            repository.CreateBlog("title", "username", "password", hostName, subfolder1);
            repository.CreateBlog("title", "username", "password", hostName, subfolder2);

            Blog info = repository.GetBlog(hostName, subfolder1);
            Assert.IsNotNull(info, "Could not find the blog with the unique hostName & subfolder combination.");
            Assert.AreEqual(info.Subfolder, subfolder1, "Oops! Looks like we found the wrong Blog!");

            info = repository.GetBlog(hostName, subfolder2);
            Assert.IsNotNull(info, "Could not find the blog with the unique hostName & subfolder combination.");
            Assert.AreEqual(info.Subfolder, subfolder2, "Oops! Looks like we found the wrong Blog!");
        }

        [DatabaseIntegrationTestMethod]
        public void GetBlogInfoDoesNotFindBlogWithWrongSubfolderInMultiBlogSystem()
        {
            var repository = new DatabaseObjectProvider();
            string subfolder1 = UnitTestHelper.GenerateUniqueString();
            string subfolder2 = UnitTestHelper.GenerateUniqueString();
            repository.CreateBlog("title", "username", "password", hostName, subfolder1);
            repository.CreateBlog("title", "username", "password", hostName, subfolder2);

            Blog info = repository.GetBlog(hostName, string.Empty);
            Assert.IsNull(info, "Hmm... Looks like found a blog using too generic of search criteria.");
        }


        [DatabaseIntegrationTestMethod]
        public void SettingShowEmailAddressInRssFlagDoesntChangeOtherFlags()
        {
            var repository = new DatabaseObjectProvider();
            repository.CreateBlog("title", "username", "password", hostName, string.Empty);
            Blog info = repository.GetBlog(hostName, string.Empty);
            bool test = info.IsAggregated;
            info.ShowEmailAddressInRss = false;
            repository.UpdateConfigData(info);
            info = repository.GetBlog(hostName, string.Empty);

            Assert.AreEqual(test, info.IsAggregated);
        }


        [DatabaseIntegrationTestMethod]
        public void GetBlogInfoLoadsOpenIDSettings()
        {
            var repository = new DatabaseObjectProvider();
            repository.CreateBlog("title", "username", "password", hostName, string.Empty);

            Blog info = repository.GetBlog(hostName, string.Empty);
            info.OpenIdServer = "http://server.example.com/";
            info.OpenIdDelegate = "http://delegate.example.com/";
            repository.UpdateConfigData(info);
            info = repository.GetBlog(hostName, string.Empty);

            Assert.AreEqual("http://server.example.com/", info.OpenIdServer);
            Assert.AreEqual("http://delegate.example.com/", info.OpenIdDelegate);
        }

        /// <summary>
        /// Sets the up test class.  This is called once for 
        /// this test class before all the tests run.
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            //Confirm app settings
            UnitTestHelper.AssertAppSettings();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            hostName = UnitTestHelper.GenerateUniqueString();
            UnitTestHelper.SetHttpContextWithBlogRequest(hostName, "MyBlog");
        }
    }
}