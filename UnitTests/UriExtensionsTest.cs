using System;
using FluentAssertions;
using NUnit.Framework;

namespace TypedRest
{
    [TestFixture]
    public class UriExtensionsTest
    {
        [Test]
        public void TestEnsureTrailingSlashRelativeUnrooted()
        {
            new Uri("test", UriKind.Relative).EnsureTrailingSlash()
                .Should().Be(new Uri("test/", UriKind.Relative));
        }

        [Test]
        public void TestEnsureTrailingSlashRelativeRooted()
        {
            new Uri("/test", UriKind.Relative).EnsureTrailingSlash()
                .Should().Be(new Uri("/test/", UriKind.Relative));
        }

        [Test]
        public void TestEnsureTrailingSlashAbsolute()
        {
            new Uri("http://localhost/test", UriKind.Absolute).EnsureTrailingSlash()
                .Should().Be(new Uri("http://localhost/test/", UriKind.Absolute));
        }

        [Test]
        public void TestEnsureTrailingSlashRelativeUnrootedWithQuery()
        {
            new Uri("test?x=1", UriKind.Relative).EnsureTrailingSlash()
                .Should().Be(new Uri("test/?x=1", UriKind.Relative));
        }

        [Test]
        public void TestEnsureTrailingSlashRelativeRootedWithQuery()
        {
            new Uri("/test?x=1", UriKind.Relative).EnsureTrailingSlash()
                .Should().Be(new Uri("/test/?x=1", UriKind.Relative));
        }

        [Test]
        public void TestEnsureTrailingSlashAbsoluteWithQuery()
        {
            new Uri("http://localhost/test?x=1", UriKind.Absolute).EnsureTrailingSlash()
                .Should().Be(new Uri("http://localhost/test/?x=1", UriKind.Absolute));
        }
    }
}