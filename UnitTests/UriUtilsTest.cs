using System;
using FluentAssertions;
using NUnit.Framework;

namespace TypedRest
{
    [TestFixture]
    public class UriUtilsTest
    {
        [Test]
        public void TestEnsureTrailingSlashRelative()
        {
            new Uri("test", UriKind.Relative).EnsureTrailingSlash()
                .Should().Be(new Uri("test/", UriKind.Relative));
        }

        [Test]
        public void TestEnsureTrailingSlashAbsolute()
        {
            new Uri("http://localhost/test", UriKind.Absolute).EnsureTrailingSlash()
                .Should().Be(new Uri("http://localhost/test/", UriKind.Absolute));
        }
    }
}