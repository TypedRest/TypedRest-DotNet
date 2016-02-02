using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    [TestFixture]
    public class CustomEndpointTest : EndpointTestBase
    {
        private CustomEndpoint _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new CustomEndpoint(EntryEndpoint, "endpoint");
        }

        [Test]
        public async Task TestAllowHeader()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new StringContent("") {Headers = {Allow = {HttpMethod.Put.Method, HttpMethod.Post.Method}}});

            await _endpoint.GetAsync();

            _endpoint.IsVerbAllowed(HttpMethod.Put.Method).Should().BeTrue();
            _endpoint.IsVerbAllowed(HttpMethod.Post.Method).Should().BeTrue();
            _endpoint.IsVerbAllowed(HttpMethod.Delete.Method).Should().BeFalse();
        }

        [Test]
        public async Task TestLink()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Headers =
                    {
                        {"Link", "<a>; rel=target1"},
                        {"Link", "<b>; rel=target2"}
                    }
                });

            await _endpoint.GetAsync();

            _endpoint.Link("target1").Should().Be(new Uri(_endpoint.Uri, "a"));
            _endpoint.Link("target2").Should().Be(new Uri(_endpoint.Uri, "b"));
        }

        [Test]
        public void TestLinkLazy()
        {
            Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Headers =
                    {
                        {"Link", "<a>; rel=target1"},
                        {"Link", "<b>; rel=target2"}
                    }
                });

            _endpoint.Link("target1").Should().Be(new Uri(_endpoint.Uri, "a"));
            _endpoint.Link("target2").Should().Be(new Uri(_endpoint.Uri, "b"));
        }

        [Test]
        public void TestLinkException()
        {
            Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Headers = {{"Link", "<a>; rel=target1"}}
                });

            _endpoint.Invoking(x => x.Link("target2")).ShouldThrow<KeyNotFoundException>();
        }

        [Test]
        public async Task TestGetLinks()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Headers =
                    {
                        {"Link", "<target1>; rel=notify"},
                        {"Link", "<target2>; rel=notify"}
                    }
                });

            await _endpoint.GetAsync();

            _endpoint.GetLinks("notify").Should().BeEquivalentTo(
                new Uri(_endpoint.Uri, "target1"),
                new Uri(_endpoint.Uri, "target2"));
        }

        [Test]
        public async Task TestLinkTemplate()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Headers =
                    {
                        {"Link", "<a>; rel=target1; templated=true"}
                    }
                });

            await _endpoint.GetAsync();

            _endpoint.LinkTemplate("target1").ToString().Should().Be("a");
            _endpoint.LinkTemplate("target2").Should().BeNull();
        }

        private class CustomEndpoint : EndpointBase
        {
            public CustomEndpoint(IEndpoint parent, string relativeUri) : base(parent, relativeUri)
            {
            }

            public async Task GetAsync()
            {
                await HandleResponseAsync(HttpClient.GetAsync(Uri));
            }

            public new bool? IsVerbAllowed(string verb)
            {
                return base.IsVerbAllowed(verb);
            }
        }
    }
}