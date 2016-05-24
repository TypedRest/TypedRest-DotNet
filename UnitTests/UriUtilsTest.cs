using System;
using System.Collections.Generic;
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

        [Test]
        public void TestQueryParametersWithAnonymousObject()
        {
            new Uri("http://localhost/test", UriKind.Absolute)
                .WithQueryParameters(new { email = "john@contoso.com", name = "john doe" }).ToString()
                .Should().Be("http://localhost/test?email=john@contoso.com&name=john+doe");
        }

        [Test]
        public void TestQueryParametersWithDictionary()
        {
            IDictionary<string,string> queryParams = new Dictionary<string, string>();
            queryParams.Add("email", "john@contoso.com");
            queryParams.Add("name", "john doe");

            new Uri("http://localhost/test", UriKind.Absolute)
                .WithQueryParameters(queryParams).ToString()
                .Should().Be("http://localhost/test?email=john@contoso.com&name=john+doe");
        }
    }
}