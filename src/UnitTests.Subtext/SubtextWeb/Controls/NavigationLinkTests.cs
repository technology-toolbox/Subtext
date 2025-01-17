﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Subtext.Web.Controls;

namespace UnitTests.Subtext.SubtextWeb.Controls
{
    [TestClass]
    public class NavigationLinkTests
    {
        [TestMethod]
        public void IsRequestForSamePage_WithRequestForRootAndNavigateUrlAsSlash_ReturnsTrue()
        {
            // arrange
            var navigationLink = new NavigationLink();

            // act
            bool isSamePage = navigationLink.IsRequestForSamePage("/", "/");

            // assert
            Assert.IsTrue(isSamePage);
        }

        [TestMethod]
        public void IsRequestForSamePage_WithRequestForRootAndNavigateUrlAsDefaultAspx_ReturnsTrue()
        {
            // arrange
            var navigationLink = new NavigationLink();

            // act
            bool isSamePage = navigationLink.IsRequestForSamePage("/Default.aspx", "/");

            // assert
            Assert.IsTrue(isSamePage);
        }

        [TestMethod]
        public void IsRequestForSamePage_WithRequestForDefaultAspxRootAndNavigateUrlAsSlash_ReturnsTrue()
        {
            // arrange
            var navigationLink = new NavigationLink { NavigateUrl = "~/" };

            // act
            bool isSamePage = navigationLink.IsRequestForSamePage("/", "/Default.aspx");

            // assert
            Assert.IsTrue(isSamePage);
        }

        [TestMethod]
        public void IsRequestForSamePage_WithRequestForDirectoryWithMatchingNavigateUrl_ReturnsTrue()
        {
            // arrange
            var navigationLink = new NavigationLink();

            // act
            bool isSamePage = navigationLink.IsRequestForSamePage("/Foo/Bar/", "/Foo/Bar/");

            // assert
            Assert.IsTrue(isSamePage);
        }

        [TestMethod]
        public void IsRequestForSamePage_WithDefaultAspxRequestForDirectoryWithMatchingNavigateUrl_ReturnsTrue()
        {
            // arrange
            var navigationLink = new NavigationLink();

            // act
            bool isSamePage = navigationLink.IsRequestForSamePage("/Foo/Bar/", "/Foo/Bar/Default.aspx");

            // assert
            Assert.IsTrue(isSamePage);
        }

        [TestMethod]
        public void IsRequestForSamePage_WithRequestForDirectoryWithMatchingDefaultAspxNavigateUrl_ReturnsTrue()
        {
            // arrange
            var navigationLink = new NavigationLink();

            // act
            bool isSamePage = navigationLink.IsRequestForSamePage("/Foo/Bar/Default.aspx", "/Foo/Bar/");

            // assert
            Assert.IsTrue(isSamePage);
        }
    }
}
