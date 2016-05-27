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
                        {"Link", "<a>; rel=target1, <b>; rel=target2"}
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
                        {"Link", "<a>; rel=target1, <b>; rel=target2"}
                    }
                });

            _endpoint.Link("target1").Should().Be(new Uri(_endpoint.Uri, "a"));
            _endpoint.Link("target2").Should().Be(new Uri(_endpoint.Uri, "b"));
        }

        [Test]
        public async Task TestLinkAbsolute()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Headers =
                    {
                        {"Link", "<http://localhost/b>; rel=target1"}
                    }
                });

            await _endpoint.GetAsync();

            _endpoint.Link("target1").Should().Be(new Uri("http://localhost/b"));
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
                        {"Link", "<target1>; rel=notify, <target2>; rel=notify"},
                        {"Link", "<target3>; rel=notify"}
                    }
                });

            await _endpoint.GetAsync();

            _endpoint.GetLinks("notify").Should().BeEquivalentTo(
                new Uri(_endpoint.Uri, "target1"),
                new Uri(_endpoint.Uri, "target2"),
                new Uri(_endpoint.Uri, "target3"));
        }

        [Test]
        public async Task TestGetLinksWithTitles()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent)
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
                {new Uri(_endpoint.Uri, "target1"), "Title"},
                {new Uri(_endpoint.Uri, "target2"), null},
            });
        }

        [Test, Ignore("Escaping in Link headers is not implemented yet.")]
        public async Task TestGetLinksWithTitlesEscaping()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Headers =
                    {
                        {"Link", "<target1>; rel=child; title=\"Title 1\", <target2>; rel=child"}
                    }
                });

            await _endpoint.GetAsync();

            _endpoint.GetLinksWithTitles("child").Should().Equal(new Dictionary<Uri, string>
            {
                {new Uri(_endpoint.Uri, "target1"), "Title 1"},
                {new Uri(_endpoint.Uri, "target2"), null}
            });
        }

        [Test]
        public void TestDefaultLink()
        {
            _endpoint.AddDefaultLink("target1", rel: "child", title: "Title 1");
            _endpoint.AddDefaultLink("target2", rel: "child");

            _endpoint.GetLinksWithTitles("child").Should().Equal(new Dictionary<Uri, string>
            {
                {new Uri(_endpoint.Uri, "target1"), "Title 1"},
                {new Uri(_endpoint.Uri, "target2"), null}
            });
        }

        [Test]
        public async Task TestLinkTemplate()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Headers =
                    {
                        {"Link", "<a{?x}>; rel=child; templated=true"}
                    }
                });

            await _endpoint.GetAsync();

            _endpoint.LinkTemplate("child").ToString().Should().Be("a{?x}");
        }

        [Test]
        public async Task TestLinkTemplateResolve()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Headers =
                    {
                        {"Link", "<a{?x}>; rel=child; templated=true"}
                    }
                });

            await _endpoint.GetAsync();

            _endpoint.LinkTemplate("child", new {x = 1}).Should().Be(new Uri(_endpoint.Uri, "a?x=1"));
        }

        [Test]
        public async Task TestLinkTemplateResolveAbsolute()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Headers =
                    {
                        {"Link", "<http://localhost/b{?x}>; rel=child; templated=true"}
                    }
                });

            await _endpoint.GetAsync();

            _endpoint.LinkTemplate("child", new {x = 1}).Should().Be(new Uri("http://localhost/b?x=1"));
        }

        [Test(Description = "Proves bugfix for issue: https://github.com/1and1/TypedRest-DotNet/issues/9")]
        public async Task GetLinkTemplateWithQueryParams()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Headers =
                    {
                        {"Link", "<http://localhost/b{?x,y}>; rel=search; templated=true"}
                    }
                });

            await _endpoint.GetAsync();

            _endpoint.LinkTemplate("search", new {x = 1, y = 2}).Should().Be(new Uri("http://localhost/b?x=1&y=2"));
        }

        [Test]
        public void TestLinkTemplateException()
        {
            Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Headers =
                    {
                        {"Link", "<a>; rel=child; templated=true"}
                    }
                });

            _endpoint.Invoking(x => x.LinkTemplate("child2")).ShouldThrow<KeyNotFoundException>();
        }

        [Test]
        public async Task TestLinkBody()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(JsonMime, "{\"links\": {"
                                   + "  \"single\": {\"href\": \"a\"},"
                                   +
                                   "  \"collection\": [{\"href\": \"b\", \"title\": \"Title 1\"},{\"href\": \"c\"},true,{\"something\":false}],"
                                   + "  \"template\": {\"href\": \"{id}\",\"templated\": true}"
                                   + "}}");

            await _endpoint.GetAsync();

            _endpoint.Link("single").Should().Be(new Uri(_endpoint.Uri, "a"));
            _endpoint.GetLinks("collection").Should().BeEquivalentTo(
                new Uri(_endpoint.Uri, "b"),
                new Uri(_endpoint.Uri, "c"));
            _endpoint.GetLinksWithTitles("collection").Should().Equal(new Dictionary<Uri, string>
            {
                {new Uri(_endpoint.Uri, "b"), "Title 1"},
                {new Uri(_endpoint.Uri, "c"), null},
            });
            _endpoint.LinkTemplate("template").ToString().Should().Be("{id}");
        }

        [Test]
        public void TestDefaultLinkTemplate()
        {
            _endpoint.AddDefaultLinkTemplate("a", rel: "child");

            _endpoint.LinkTemplate("child").ToString().Should().Be("a");
        }

        [Test]
        public void TestEnsureTrailingSlashOnParentUri()
        {
            new ActionEndpoint(_endpoint, "subresource").Uri.Should().Be(new Uri("http://localhost/subresource"));
            new ActionEndpoint(_endpoint, "subresource", ensureTrailingSlashOnParentUri: true).Uri.Should().Be(new Uri("http://localhost/endpoint/subresource"));
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

        [Test]
        public async Task TestErrorHandling()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.Conflict));

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

        [Test]
        public async Task TestErrorHandlingNoContentType()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage(HttpStatusCode.Conflict) {Content = new ByteArrayContent(new byte[0])});

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