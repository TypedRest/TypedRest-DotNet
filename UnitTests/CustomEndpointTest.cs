using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Xunit;

namespace TypedRest
{
    [Collection("Endpoint")]
    public class CustomEndpointTest : EndpointTestBase
    {
        private readonly CustomEndpoint _endpoint;

        public CustomEndpointTest()
        {
            _endpoint = new CustomEndpoint(EntryEndpoint, "endpoint");
        }

        [Fact]
        public async Task TestAllowHeader()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new StringContent("") {Headers = {Allow = {HttpMethod.Put.Method, HttpMethod.Post.Method}}});

            await _endpoint.GetAsync();

            _endpoint.IsMethodAllowed(HttpMethod.Put).Should().BeTrue();
            _endpoint.IsMethodAllowed(HttpMethod.Post).Should().BeTrue();
            _endpoint.IsMethodAllowed(HttpMethod.Delete).Should().BeFalse();
        }

        [Fact]
        public async Task TestLink()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.NoContent)
                 {
                     Headers =
                     {
                         {"Link", "<a>; rel=target1, <b>; rel=target2"}
                     }
                 });

            await _endpoint.GetAsync();

            _endpoint.Link("target1").Should().Be(new Uri("http://localhost/a"));
            _endpoint.Link("target2").Should().Be(new Uri("http://localhost/b"));
        }

        [Fact]
        public void TestLinkLazy()
        {
            Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.NoContent)
                 {
                     Headers =
                     {
                         {"Link", "<a>; rel=target1, <b>; rel=target2"}
                     }
                 });

            _endpoint.Link("target1").Should().Be(new Uri("http://localhost/a"));
            _endpoint.Link("target2").Should().Be(new Uri("http://localhost/b"));
        }

        [Fact]
        public async Task TestLinkAbsolute()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.NoContent)
                 {
                     Headers =
                     {
                         {"Link", "<http://localhost/b>; rel=target1"}
                     }
                 });

            await _endpoint.GetAsync();

            _endpoint.Link("target1").Should().Be(new Uri("http://localhost/b"));
        }

        [Fact]
        public void TestLinkException()
        {
            Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.NoContent)
                 {
                     Headers = {{"Link", "<a>; rel=target1"}}
                 });

            Assert.Throws<KeyNotFoundException>(() => _endpoint.Link("target2"));
        }

        [Fact]
        public async Task TestGetLinks()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.NoContent)
                 {
                     Headers =
                     {
                         {"Link", "<target1>; rel=notify, <target2>; rel=notify"},
                         {"Link", "<target3>; rel=notify"}
                     }
                 });

            await _endpoint.GetAsync();

            _endpoint.GetLinks("notify").Should().BeEquivalentTo(
                new Uri("http://localhost/target1"),
                new Uri("http://localhost/target2"),
                new Uri("http://localhost/target3"));
        }

        [Fact]
        public async Task TestGetLinksWithTitles()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.NoContent)
                 {
                     Headers =
                     {
                         {"Link", "<target1>; rel=child; title=Title"},
                         {"Link", "<target2>; rel=child"}
                     }
                 });

            await _endpoint.GetAsync();

            _endpoint.GetLinksWithTitles("child").Should().Equal(new Dictionary<Uri, string>
            {
                {new Uri("http://localhost/target1"), "Title"},
                {new Uri("http://localhost/target2"), null}
            });
        }

        [Fact]
        public async Task TestGetLinksWithTitlesEscaping()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.NoContent)
                 {
                     Headers =
                     {
                         {"Link", "<target1>; rel=child; title=\"Title 1\", <target2>; rel=child"}
                     }
                 });

            await _endpoint.GetAsync();

            _endpoint.GetLinksWithTitles("child").Should().Equal(new Dictionary<Uri, string>
            {
                {new Uri("http://localhost/target1"), "Title 1"},
                {new Uri("http://localhost/target2"), null}
            });
        }

        [Fact]
        public void TestSetDefaultLink()
        {
            _endpoint.SetDefaultLink(rel: "child", hrefs: new[] {"target1", "target2"});

            _endpoint.GetLinksWithTitles("child").Should().Equal(new Dictionary<Uri, string>
            {
                {new Uri("http://localhost/target1"), null},
                {new Uri("http://localhost/target2"), null}
            });
        }

        [Fact]
        public async Task TestLinkTemplate()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.NoContent)
                 {
                     Headers =
                     {
                         {"Link", "<a{?x}>; rel=child; templated=true"}
                     }
                 });

            await _endpoint.GetAsync();

            _endpoint.LinkTemplate("child").ToString().Should().Be("a{?x}");
        }

        [Fact]
        public async Task TestLinkTemplateResolve()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.NoContent)
                 {
                     Headers =
                     {
                         {"Link", "<a{?x}>; rel=child; templated=true"}
                     }
                 });

            await _endpoint.GetAsync();

            _endpoint.LinkTemplate("child", new {x = 1}).Should().Be(new Uri("http://localhost/a?x=1"));
        }

        [Fact]
        public async Task TestLinkTemplateResolveAbsolute()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.NoContent)
                 {
                     Headers =
                     {
                         {"Link", "<http://localhost/b{?x}>; rel=child; templated=true"}
                     }
                 });

            await _endpoint.GetAsync();

            _endpoint.LinkTemplate("child", new {x = 1}).Should().Be(new Uri("http://localhost/b?x=1"));
        }

        [Fact]
        public async Task TestLinkTemplateResolveEscaping()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.NoContent)
                 {
                     Headers =
                     {
                         {"Link", "<http://localhost/b{?x,y}>; rel=search; templated=true"}
                     }
                 });

            await _endpoint.GetAsync();

            _endpoint.LinkTemplate("search", new {x = 1, y = 2}).Should().Be(new Uri("http://localhost/b?x=1&y=2"));
        }

        [Fact]
        public void TestLinkTemplateException()
        {
            Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.NoContent)
                 {
                     Headers =
                     {
                         {"Link", "<a>; rel=child; templated=true"}
                     }
                 });

            Assert.Throws<KeyNotFoundException>(() => _endpoint.LinkTemplate("child2"));
        }

        [Fact]
        public async Task TestLinkBody()
        {
            string body = JsonConvert.SerializeObject(new
            {
                links = new
                {
                    single = new {href = "a"},
                    collection = new object[] {new {href = "b", title = "Title 1"}, new {href = "c"}, true, new {something = false}},
                    template = new {href = "{id}", templated = true}
                }
            });
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint").Respond(JsonMime, body);

            await _endpoint.GetAsync();

            _endpoint.Link("single").Should().Be(new Uri("http://localhost/a"));
            _endpoint.GetLinks("collection").Should().BeEquivalentTo(
                new Uri("http://localhost/b"),
                new Uri("http://localhost/c"));
            _endpoint.GetLinksWithTitles("collection").Should().Equal(new Dictionary<Uri, string>
            {
                {new Uri("http://localhost/b"), "Title 1"},
                {new Uri("http://localhost/c"), null}
            });
            _endpoint.LinkTemplate("template").ToString().Should().Be("{id}");
        }

        [Fact]
        public void TestSetDefaultLinkTemplate()
        {
            _endpoint.SetDefaultLinkTemplate(rel: "child", href: "a");

            _endpoint.LinkTemplate("child").ToString().Should().Be("a");
        }

        [Fact]
        public void TestEnsureTrailingSlashOnReferrerUri()
        {
            new ActionEndpoint(_endpoint, "subresource").Uri.Should().Be(new Uri("http://localhost/subresource"));
            new ActionEndpoint(_endpoint, "./subresource").Uri.Should().Be(new Uri("http://localhost/endpoint/subresource"));
        }

        private class CustomEndpoint : EndpointBase
        {
            public CustomEndpoint(IEndpoint referrer, string relativeUri)
                : base(referrer, relativeUri)
            {}

            public Task GetAsync() => HandleResponseAsync(HttpClient.GetAsync(Uri));

            public new bool? IsMethodAllowed(HttpMethod method) => base.IsMethodAllowed(method);
        }

        [Fact]
        public async Task TestErrorHandling()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.Conflict));

            bool thrown = false;
            try
            {
                await _endpoint.GetAsync();
            }
            catch (InvalidOperationException ex)
            {
                ex.Message.Should().Be("http://localhost/endpoint responded with 409 Conflict");
                thrown = true;
            }
            thrown.Should().BeTrue(because: $"{nameof(InvalidOperationException)} should be thrown");
        }

        [Fact]
        public async Task TestErrorHandlingNoContentType()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.Conflict) {Content = new ByteArrayContent(new byte[0])});

            bool thrown = false;
            try
            {
                await _endpoint.GetAsync();
            }
            catch (InvalidOperationException ex)
            {
                ex.Message.Should().Be("http://localhost/endpoint responded with 409 Conflict");
                thrown = true;
            }
            thrown.Should().BeTrue(because: $"{nameof(InvalidOperationException)} should be thrown");
        }
    }
}
