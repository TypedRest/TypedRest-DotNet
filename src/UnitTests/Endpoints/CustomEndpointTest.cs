using Newtonsoft.Json;
using TypedRest.Endpoints.Rpc;

namespace TypedRest.Endpoints;

[Collection("Endpoint")]
public class CustomEndpointTest : EndpointTestBase
{
    private readonly CustomEndpoint _endpoint;

    public CustomEndpointTest()
    {
        _endpoint = new(EntryEndpoint, "endpoint");
    }

    [Fact]
    public async Task TestAcceptHeader()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("Accept", JsonMime)
            .Respond(_ => new StringContent("{}"));

        await _endpoint.GetAsync();
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
            .Respond(_ => new(HttpStatusCode.NoContent)
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
            .Respond(_ => new(HttpStatusCode.NoContent)
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
            .Respond(_ => new(HttpStatusCode.NoContent)
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
            .Respond(_ => new(HttpStatusCode.NoContent)
             {
                 Headers = {{"Link", "<a>; rel=target1"}}
             });

        Assert.Throws<KeyNotFoundException>(() => _endpoint.Link("target2"));
    }

    [Fact]
    public async Task TestGetLinks()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.NoContent)
             {
                 Headers =
                 {
                     {"Link", "<target1>; rel=child; title=Title"},
                     {"Link", "<target2>; rel=child"}
                 }
             });

        await _endpoint.GetAsync();

        _endpoint.GetLinks("child").Should().BeEquivalentTo(new[]
        {
            (new Uri("http://localhost/target1"), "Title"),
            (new Uri("http://localhost/target2"), (string?)null)
        });
    }

    [Fact]
    public async Task TestGetLinksEscaping()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.NoContent)
             {
                 Headers =
                 {
                     {"Link", "<target1>; rel=child; title=\"Title,= 1\", <target2>; rel=child"}
                 }
             });

        await _endpoint.GetAsync();

        _endpoint.GetLinks("child").Should().BeEquivalentTo(new[]
        {
            (new Uri("http://localhost/target1"), "Title,= 1"),
            (new Uri("http://localhost/target2"), (string?)null)
        });
    }

    [Fact]
    public void TestSetDefaultLink()
    {
        _endpoint.SetDefaultLink(rel: "child", "target");

        _endpoint.Link("child").Should().Be(new Uri("http://localhost/target"));
    }

    [Fact]
    public async Task TestLinkTemplate()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.NoContent)
             {
                 Headers =
                 {
                     {"Link", "<a{?x}>; rel=child; templated=true"}
                 }
             });

        await _endpoint.GetAsync();

        _endpoint.GetLinkTemplate("child").ToString().Should().Be("a{?x}");
    }

    [Fact]
    public async Task TestLinkTemplateResolve()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.NoContent)
             {
                 Headers =
                 {
                     {"Link", "<a{?x}>; rel=child; templated=true"}
                 }
             });

        await _endpoint.GetAsync();

        _endpoint.LinkTemplate("child", new {x = "1"}).Should().Be(new Uri("http://localhost/a?x=1"));
    }

    [Fact]
    public async Task TestLinkTemplateResolveAbsolute()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.NoContent)
             {
                 Headers =
                 {
                     {"Link", "<http://localhost/b{?x}>; rel=child; templated=true"}
                 }
             });

        await _endpoint.GetAsync();

        _endpoint.LinkTemplate("child", new {x = "1"}).Should().Be(new Uri("http://localhost/b?x=1"));
    }

    [Fact]
    public async Task TestLinkTemplateResolveQuery()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.NoContent)
             {
                 Headers =
                 {
                     {"Link", "<http://localhost/b{?x,y}>; rel=search; templated=true"}
                 }
             });

        await _endpoint.GetAsync();

        _endpoint.LinkTemplate("search", new {x = "1", y = "2"}).Should().Be(new Uri("http://localhost/b?x=1&y=2"));
    }

    [Fact]
    public void TestLinkTemplateException()
    {
        Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.NoContent)
             {
                 Headers =
                 {
                     {"Link", "<a>; rel=child; templated=true"}
                 }
             });

        Assert.Throws<KeyNotFoundException>(() => _endpoint.GetLinkTemplate("child2"));
    }

    [Fact]
    public async Task TestLinkHal()
    {
        string body = JsonConvert.SerializeObject(new
        {
            _links = new
            {
                single = new {href = "a"},
                collection = new object[] {new {href = "b", title = "Title 1"}, new {href = "c"}},
                template = new {href = "{id}", templated = true}
            }
        });
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint").Respond("application/hal+json", body);

        await _endpoint.GetAsync();

        _endpoint.Link("single").Should().Be(new Uri("http://localhost/a"));
        _endpoint.GetLinks("collection").Should().BeEquivalentTo(new[]
        {
            (new Uri("http://localhost/b"), "Title 1"),
            (new Uri("http://localhost/c"), (string?)null)
        });
        _endpoint.GetLinkTemplate("template").ToString().Should().Be("{id}");
    }

    [Fact]
    public void TestSetDefaultLinkTemplate()
    {
        _endpoint.SetDefaultLinkTemplate(rel: "child", href: "a");

        _endpoint.GetLinkTemplate("child").ToString().Should().Be("a");
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

        public async Task GetAsync()
            => await HandleAsync(() => HttpClient.GetAsync(Uri)).NoContext();

        public new bool? IsMethodAllowed(HttpMethod method)
            => base.IsMethodAllowed(method);
    }

    [Fact]
    public async Task TestErrorHandlingWithNoContent()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.Conflict));

        await _endpoint.Awaiting(x => x.GetAsync())
                       .Should().ThrowAsync<InvalidOperationException>()
                       .WithMessage("http://localhost/endpoint responded with 409 Conflict");
    }

    [Fact]
    public async Task TestErrorHandlingWithMessage()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.Conflict)
             {
                 Content = new StringContent("{\"message\":\"my message\"}")
                 {
                     Headers = {ContentType = MediaTypeHeaderValue.Parse(JsonMime)}
                 }
             });

        await _endpoint.Awaiting(x => x.GetAsync())
                       .Should().ThrowAsync<InvalidOperationException>()
                       .WithMessage("my message");
    }

    [Fact]
    public async Task TestErrorHandlingWithArray()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.Conflict)
             {
                 Content = new StringContent("[{\"message\":\"my message\"}]")
                 {
                     Headers = {ContentType = MediaTypeHeaderValue.Parse(JsonMime)}
                 }
             });

        await _endpoint.Awaiting(x => x.GetAsync())
                       .Should().ThrowAsync<InvalidOperationException>()
                       .WithMessage("http://localhost/endpoint responded with 409 Conflict");
    }

    [Fact]
    public void TestErrorHandlingWithUnknownContentType()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.Conflict) {Content = new ByteArrayContent(new byte[0])});

        _endpoint.Awaiting(x => x.GetAsync())
                 .Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("http://localhost/endpoint responded with 409 Conflict");
    }
}