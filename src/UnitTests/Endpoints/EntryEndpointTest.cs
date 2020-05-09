using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using FluentAssertions;
using Xunit;

namespace TypedRest.Endpoints
{
    public class EntryEndpointTest
    {
        [Fact]
        public void TestUriFromHttpClient()
        {
            var endpoint = new EntryEndpoint(new HttpClient {BaseAddress = new Uri("http://localhost")});
            endpoint.Uri.Should().Be( new Uri("http://localhost/"));
        }

        [Fact]
        public void TestBasicAuth()
        {
            var endpoint = new EntryEndpoint(new Uri("http://localhost/"), new NetworkCredential("user", "password"));
            endpoint.HttpClient.DefaultRequestHeaders.Authorization
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
    }
}
