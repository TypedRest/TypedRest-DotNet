using System;
using FluentAssertions;
using Xunit;

namespace TypedRest
{
    public class UriExtensionsTest
    {
        [Fact]
        public void TestEnsureTrailingSlashRelativeUnrooted()
            => new Uri("test", UriKind.Relative).EnsureTrailingSlash()
                .Should().Be(new Uri("test/", UriKind.Relative));

        [Fact]
        public void TestEnsureTrailingSlashRelativeRooted()
            => new Uri("/test", UriKind.Relative).EnsureTrailingSlash()
                .Should().Be(new Uri("/test/", UriKind.Relative));

        [Fact]
        public void TestEnsureTrailingSlashAbsolute()
            => new Uri("http://localhost/test", UriKind.Absolute).EnsureTrailingSlash()
                .Should().Be(new Uri("http://localhost/test/", UriKind.Absolute));

        [Fact]
        public void TestEnsureTrailingSlashRelativeUnrootedWithQuery()
            => new Uri("test?x=1", UriKind.Relative).EnsureTrailingSlash()
                .Should().Be(new Uri("test/?x=1", UriKind.Relative));

        [Fact]
        public void TestEnsureTrailingSlashRelativeRootedWithQuery()
            => new Uri("/test?x=1", UriKind.Relative).EnsureTrailingSlash()
                .Should().Be(new Uri("/test/?x=1", UriKind.Relative));

        [Fact]
        public void TestEnsureTrailingSlashAbsoluteWithQuery()
            => new Uri("http://localhost/test?x=1", UriKind.Absolute).EnsureTrailingSlash()
                .Should().Be(new Uri("http://localhost/test/?x=1", UriKind.Absolute));
    }
}