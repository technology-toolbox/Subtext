using System;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Subtext.Framework;
using Subtext.Framework.Infrastructure.Installation;
using Subtext.Framework.Web.HttpModules;
using Subtext.Framework.Services;

namespace UnitTests.Subtext.Framework.Web.HttpModules
{
    [TestClass]
    public class InstallationCheckModuleTests
    {
        private static HostInfo CreateHostInfo()
        {
            return new HostInfo(new NameValueCollection());
        }

        [TestMethod]
        public void GetInstallationRedirectUrl_ForStaticFiles_ReturnsNull()
        {
            // arrange
            var module = new InstallationCheckModule(new Mock<IInstallationManager>().Object, new LazyNotNull<HostInfo>(() => null));
            var blogRequest = new BlogRequest("localhost", string.Empty, new Uri("http://localhost/whatever/foo.jpg"),
                                              true, RequestLocation.Blog, "/");
            // act
            string redirectUrl = module.GetInstallationRedirectUrl(blogRequest);

            // assert
            Assert.IsNull(redirectUrl);
        }

        [TestMethod]
        public void GetInstallationRedirectUrl_WhenHostInfoNull_ReturnsInstallDirectory()
        {
            // arrange
            var module = new InstallationCheckModule(new Mock<IInstallationManager>().Object, new LazyNotNull<HostInfo>(() => null));
            var blogRequest = new BlogRequest("localhost", string.Empty, new Uri("http://localhost/foo.aspx"), true,
                                              RequestLocation.Blog, "/");

            // act
            string redirectUrl = module.GetInstallationRedirectUrl(blogRequest);

            // assert
            Assert.AreEqual("~/install/default.aspx", redirectUrl);
        }

        [TestMethod]
        public void GetInstallationRedirectUrl_WhenHostInfoNullButInInstallDirAndNoUpgradeIsRequired_ReturnsNull()
        {
            // arrange
            var installManager = new Mock<IInstallationManager>();
            installManager.Setup(m => m.InstallationActionRequired(It.IsAny<Version>(), null)).Returns(false);
            var module = new InstallationCheckModule(installManager.Object, new LazyNotNull<HostInfo>(() => null));
            var blogRequest = new BlogRequest("localhost", string.Empty, new Uri("http://localhost/Install/foo.aspx"),
                                              true, RequestLocation.Installation, "/");

            // act
            string redirectUrl = module.GetInstallationRedirectUrl(blogRequest);

            // assert
            Assert.IsNull(redirectUrl);
        }

        [TestMethod]
        public void GetInstallationRedirectUrl_WhenHostInfoNotNullAndInstallRequiredButInInstallDirectory_ReturnsNull()
        {
            // arrange
            var installManager = new Mock<IInstallationManager>();
            installManager.Setup(m => m.InstallationActionRequired(It.IsAny<Version>(), null)).Returns(true);
            var module = new InstallationCheckModule(installManager.Object, new LazyNotNull<HostInfo>(CreateHostInfo));
            var blogRequest = new BlogRequest("localhost", string.Empty, new Uri("http://localhost/Install/foo.aspx"),
                                              true, RequestLocation.Installation, "/");

            // act
            string redirectUrl = module.GetInstallationRedirectUrl(blogRequest);

            // assert
            Assert.IsNull(redirectUrl);
        }

        [TestMethod]
        public void GetInstallationRedirectUrl_WhenHostInfoNotNullAndInstallRequiredButInHostAdminDirectory_ReturnsNull()
        {
            // arrange
            var installManager = new Mock<IInstallationManager>();
            installManager.Setup(m => m.GetInstallationStatus(It.IsAny<Version>())).Returns(
                InstallationState.NeedsInstallation);
            installManager.Setup(m => m.InstallationActionRequired(It.IsAny<Version>(), null)).Returns(true);
            var module = new InstallationCheckModule(installManager.Object, new LazyNotNull<HostInfo>(UnitTestHelper.CreateHostInfo));
            var blogRequest = new BlogRequest("localhost", string.Empty, new Uri("http://localhost/Install/foo.aspx"),
                                              true, RequestLocation.HostAdmin, "/");

            // act
            string redirectUrl = module.GetInstallationRedirectUrl(blogRequest);

            // assert
            Assert.IsNull(redirectUrl);
        }

        [TestMethod]
        public void
            GetInstallationRedirectUrl_WhenHostInfoNotNullInstallationActionRequiredAndNotInInstallDirectory_ReturnsInstallDirecotry
            ()
        {
            // arrange
            var installManager = new Mock<IInstallationManager>();
            installManager.Setup(m => m.GetInstallationStatus(It.IsAny<Version>())).Returns(
                InstallationState.NeedsInstallation);
            installManager.Setup(m => m.InstallationActionRequired(It.IsAny<Version>(), null)).Returns(true);
            var module = new InstallationCheckModule(installManager.Object, new LazyNotNull<HostInfo>(UnitTestHelper.CreateHostInfo));
            var blogRequest = new BlogRequest("localhost", string.Empty, new Uri("http://localhost/Install/foo.aspx"),
                                              true, RequestLocation.Blog, "/");

            // act
            string redirectUrl = module.GetInstallationRedirectUrl(blogRequest);

            // assert
            Assert.AreEqual("~/install/default.aspx", redirectUrl);
        }

        [TestMethod]
        public void
            GetInstallationRedirectUrl_WhenHostInfoNotNullInstallationActionRequiredAndInLoginPage_ReturnsInstallDirectory
            ()
        {
            // arrange
            var installManager = new Mock<IInstallationManager>();
            installManager.Setup(m => m.GetInstallationStatus(It.IsAny<Version>())).Returns(
                InstallationState.NeedsInstallation);
            installManager.Setup(m => m.InstallationActionRequired(It.IsAny<Version>(), null)).Returns(true);
            var module = new InstallationCheckModule(installManager.Object, new LazyNotNull<HostInfo>(UnitTestHelper.CreateHostInfo));
            var blogRequest = new BlogRequest("localhost", string.Empty, new Uri("http://localhost/Install/foo.aspx"),
                                              true, RequestLocation.LoginPage, "/");

            // act
            string redirectUrl = module.GetInstallationRedirectUrl(blogRequest);

            // assert
            Assert.AreEqual("~/install/default.aspx", redirectUrl);
        }

        [TestMethod]
        public void GetInstallationRedirectUrl_WhenUpgradeRequiredAndInLoginPage_ReturnsNull()
        {
            // arrange
            var installManager = new Mock<IInstallationManager>();
            installManager.Setup(m => m.GetInstallationStatus(It.IsAny<Version>())).Returns(
                InstallationState.NeedsUpgrade);
            installManager.Setup(m => m.InstallationActionRequired(It.IsAny<Version>(), null)).Returns(true);
            var module = new InstallationCheckModule(installManager.Object, new LazyNotNull<HostInfo>(UnitTestHelper.CreateHostInfo));
            var blogRequest = new BlogRequest("localhost", string.Empty, new Uri("http://localhost/Install/foo.aspx"),
                                              true, RequestLocation.LoginPage, "/");

            // act
            string redirectUrl = module.GetInstallationRedirectUrl(blogRequest);

            // assert
            Assert.IsNull(redirectUrl);
        }

        [TestMethod]
        public void GetInstallationRedirectUrl_WhenUpgradeRequiredAndInUpgradeDirectory_ReturnsNull()
        {
            // arrange
            var installManager = new Mock<IInstallationManager>();
            installManager.Setup(m => m.GetInstallationStatus(It.IsAny<Version>())).Returns(
                InstallationState.NeedsUpgrade);
            installManager.Setup(m => m.InstallationActionRequired(It.IsAny<Version>(), null)).Returns(true);
            var module = new InstallationCheckModule(installManager.Object, new LazyNotNull<HostInfo>(UnitTestHelper.CreateHostInfo));
            var blogRequest = new BlogRequest("localhost", string.Empty, new Uri("http://localhost/Install/foo.aspx"),
                                              true, RequestLocation.Upgrade, "/");

            // act
            string redirectUrl = module.GetInstallationRedirectUrl(blogRequest);

            // assert
            Assert.IsNull(redirectUrl);
        }

        [TestMethod]
        public void GetInstallationRedirectUrl_WhenUpgradeRequiredAndInSystemMessagesDirectory_ReturnsNull()
        {
            // arrange
            var installManager = new Mock<IInstallationManager>();
            installManager.Setup(m => m.GetInstallationStatus(It.IsAny<Version>())).Returns(
                InstallationState.NeedsUpgrade);
            installManager.Setup(m => m.InstallationActionRequired(It.IsAny<Version>(), null)).Returns(true);
            var module = new InstallationCheckModule(installManager.Object, new LazyNotNull<HostInfo>(UnitTestHelper.CreateHostInfo));
            var blogRequest = new BlogRequest("localhost", string.Empty, new Uri("http://localhost/Install/foo.aspx"),
                                              true, RequestLocation.SystemMessages, "/");

            // act
            string redirectUrl = module.GetInstallationRedirectUrl(blogRequest);

            // assert
            Assert.IsNull(redirectUrl);
        }

        [TestMethod]
        public void GetInstallationRedirectUrl_WhenUpgradeRequiredAndInHostAdminDirectory_ReturnsNull()
        {
            // arrange
            var installManager = new Mock<IInstallationManager>();
            installManager.Setup(m => m.GetInstallationStatus(It.IsAny<Version>())).Returns(
                InstallationState.NeedsUpgrade);
            installManager.Setup(m => m.InstallationActionRequired(It.IsAny<Version>(), null)).Returns(true);
            var module = new InstallationCheckModule(installManager.Object, new LazyNotNull<HostInfo>(UnitTestHelper.CreateHostInfo));
            var blogRequest = new BlogRequest("localhost", string.Empty, new Uri("http://localhost/Install/foo.aspx"),
                                              true, RequestLocation.HostAdmin, "/");

            // act
            string redirectUrl = module.GetInstallationRedirectUrl(blogRequest);

            // assert
            Assert.IsNull(redirectUrl);
        }

        [TestMethod]
        public void GetInstallationRedirectUrl_WhenUpgradeRequired_ReturnsUpgradeDirectory()
        {
            // arrange
            var installManager = new Mock<IInstallationManager>();
            installManager.Setup(m => m.GetInstallationStatus(It.IsAny<Version>())).Returns(
                InstallationState.NeedsUpgrade);
            installManager.Setup(m => m.InstallationActionRequired(It.IsAny<Version>(), null)).Returns(true);
            var module = new InstallationCheckModule(installManager.Object, new LazyNotNull<HostInfo>(UnitTestHelper.CreateHostInfo));
            var blogRequest = new BlogRequest("localhost", string.Empty, new Uri("http://localhost/Install/foo.aspx"),
                                              true, RequestLocation.Blog, "/");

            // act
            string redirectUrl = module.GetInstallationRedirectUrl(blogRequest);

            // assert
            Assert.AreEqual("~/SystemMessages/UpgradeInProgress.aspx", redirectUrl);
        }
    }
}