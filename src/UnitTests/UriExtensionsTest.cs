namespace TypedRest;

public class UriExtensionsTest
{
    [Fact]
    public void TestEnsureTrailingSlashRelativeUnrooted()
        => new Uri("test", UriKind.Relative).EnsureTrailingSlash().Should().Be(
            new Uri("test/", UriKind.Relative));

    [Fact]
    public void TestEnsureTrailingSlashRelativeRooted()
        => new Uri("/test", UriKind.Relative).EnsureTrailingSlash().Should().Be(
            new Uri("/test/", UriKind.Relative));

    [Fact]
    public void TestEnsureTrailingSlashAbsolute()
        => new Uri("http://localhost/test", UriKind.Absolute).EnsureTrailingSlash().Should().Be(
            new Uri("http://localhost/test/", UriKind.Absolute));

    [Fact]
    public void TestEnsureTrailingSlashRelativeUnrootedWithQuery()
        => new Uri("test?x=1", UriKind.Relative).EnsureTrailingSlash().Should().Be(
            new Uri("test/?x=1", UriKind.Relative));

    [Fact]
    public void TestEnsureTrailingSlashRelativeRootedWithQuery()
        => new Uri("/test?x=1", UriKind.Relative).EnsureTrailingSlash().Should().Be(
            new Uri("/test/?x=1", UriKind.Relative));

    [Fact]
    public void TestEnsureTrailingSlashAbsoluteWithQuery()
        => new Uri("http://localhost/test?x=1", UriKind.Absolute).EnsureTrailingSlash().Should().Be(
            new Uri("http://localhost/test/?x=1", UriKind.Absolute));

    [Fact]
    public void TestJoinStringNormal()
        => new Uri("http://localhost/test?x=1", UriKind.Absolute).Join(relativeUri: "subpath").Should().Be(
            new Uri("http://localhost/subpath", UriKind.Absolute));

    [Fact]
    public void TestJoinUriNormal()
        => new Uri("http://localhost/test?x=1", UriKind.Absolute).Join(relativeUri: new Uri("subpath", UriKind.Relative)).Should().Be(
            new Uri("http://localhost/subpath", UriKind.Absolute));

    [Fact]
    public void TestJoinStringWithDotSlash()
        => new Uri("http://localhost/test?x=1", UriKind.Absolute).Join(relativeUri: "./subpath").Should().Be(
            new Uri("http://localhost/test/subpath", UriKind.Absolute));

    [Fact]
    public void TestJoinUriWithDotSlash()
        => new Uri("http://localhost/test?x=1", UriKind.Absolute).Join(relativeUri: new Uri("./subpath", UriKind.Relative)).Should().Be(
            new Uri("http://localhost/test/subpath", UriKind.Absolute));
}
