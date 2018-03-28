using System;
using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Xunit;

namespace TypedRest
{
    public class EntryEndpointTest
    {
        [Fact]
        public void TestBasicAuth()
        {
            new EntryEndpoint(new Uri("http://localhost/"), new NetworkCredential("user", "password"))
                .HttpClient.DefaultRequestHeaders.Authorization
                .Should().Be(new AuthenticationHeaderValue("Basic", "dXNlcjpwYXNzd29yZA=="));
        }

        [Fact]
        public void TestBasicAuthInUri()
        {
            var endpoint = new EntryEndpoint(new Uri("http://user:password@localhost/"));
            endpoint.HttpClient.DefaultRequestHeaders.Authorization
                .Should().Be(new AuthenticationHeaderValue("Basic", "dXNlcjpwYXNzd29yZA=="));
            endpoint.Uri.Should().Be(new Uri("http://localhost/"));
        }

        [Fact]
        public void TestBearerAuth()
        {
            new EntryEndpoint(new Uri("http://localhost/"), token: "token")
                .HttpClient.DefaultRequestHeaders.Authorization
                .Should().Be(new AuthenticationHeaderValue("Bearer", "token"));
        }
    }
}
