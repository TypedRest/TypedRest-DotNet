using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace TypedRest
{
    [TestFixture, Ignore("Server mock not implemented yet")]
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
            //stubFor(get(urlEqualTo("/endpoint"))
            //    .willReturn(aResponse()
            //        .withStatus(SC_OK)
            //        .withHeader("Allow", "PUT, POST")));

            await _endpoint.GetAsync();

            _endpoint.IsVerbAllowed("PUT").Should().BeTrue();
            _endpoint.IsVerbAllowed("POST").Should().BeTrue();
            _endpoint.IsVerbAllowed("DELETE").Should().BeFalse();
        }

        [Test]
        public async Task TestLink()
        {
            //stubFor(get(urlEqualTo("/endpoint"))
            //    .willReturn(aResponse()
            //        .withStatus(SC_NO_CONTENT)
            //        .withHeader("Link", "<a>; rel=target1, <b>; rel=target2")));

            await _endpoint.GetAsync();

            _endpoint.Link("target1").Should().Be(new Uri(_endpoint.Uri, "a"));
            _endpoint.Link("target2").Should().Be(new Uri(_endpoint.Uri, "b"));
        }

        [Test]
        public void TestLinkLazy()
        {
            //stubFor(head(urlEqualTo("/endpoint"))
            //    .willReturn(aResponse()
            //        .withStatus(SC_NO_CONTENT)
            //        .withHeader("Link", "<a>; rel=target1, <b>; rel=target2")));

            _endpoint.Link("target1").Should().Be(new Uri(_endpoint.Uri, "a"));
            _endpoint.Link("target2").Should().Be(new Uri(_endpoint.Uri, "b"));
        }

        [Test]
        public async Task TestLinkException()
        {
            //stubFor(head(urlEqualTo("/endpoint"))
            //    .willReturn(aResponse()
            //        .withStatus(SC_NO_CONTENT)
            //        .withHeader("Link", "<a>; rel=target1")));

            await _endpoint.GetAsync();

            _endpoint.Invoking(x => x.Link("target2")).ShouldThrow<KeyNotFoundException>();
        }

        [Test]
        public async Task TestGetLinks()
        {
            //stubFor(get(urlEqualTo("/endpoint"))
            //    .willReturn(aResponse()
            //        .withStatus(SC_NO_CONTENT)
            //        .withHeader("Link", "<target1>; rel=notify, <target2>; rel=notify")));

            await _endpoint.GetAsync();

            _endpoint.GetLinks("notify").Should().BeEquivalentTo(
                new Uri(_endpoint.Uri, "target1"),
                new Uri(_endpoint.Uri, "target2"));
        }

        [Test]
        public async Task TestLinkTemplate()
        {
            //stubFor(get(urlEqualTo("/endpoint"))
            //    .willReturn(aResponse()
            //        .withStatus(SC_NO_CONTENT)
            //        .withHeader("Link", "<a>; rel=target1-template")));

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