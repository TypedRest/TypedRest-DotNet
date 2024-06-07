namespace TypedRest.Endpoints.Generic;

[Collection("Endpoint")]
public class CollectionEndpointTest : EndpointTestBase
{
    private readonly CollectionEndpoint<MockEntity> _endpoint;

    public CollectionEndpointTest()
    {
        _endpoint = new(EntryEndpoint, "endpoint");
    }

    [Fact]
    public void TestGetById()
    {
        _endpoint["x/y"].Uri.Should().Be(new Uri("http://localhost/endpoint/x%2Fy"));
    }

    [Fact]
    public async Task TestGetByIdWithLinkHeaderRelative()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("[]", Encoding.UTF8, JsonMime),
                 Headers =
                 {
                     {"Link", "<children{?id}>; rel=child; templated=true"}
                 }
             });
        await _endpoint.ReadAllAsync();

        _endpoint["1"]
           .Uri.Should().Be(new Uri("http://localhost/children?id=1"));
    }

    [Fact]
    public async Task TestGetByIdWithLinkHeaderAbsolute()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("[]", Encoding.UTF8, JsonMime),
                 Headers =
                 {
                     {"Link", "<http://localhost/children{?id}>; rel=child; templated=true"}
                 }
             });
        await _endpoint.ReadAllAsync();

        _endpoint["1"]
           .Uri.Should().Be(new Uri("http://localhost/children?id=1"));
    }

    [Fact]
    public void TestGetByEntity()
        => _endpoint[new MockEntity(1, "test")].Uri.Should().Be(new Uri("http://localhost/endpoint/1"));

    [Fact]
    public async Task TestGetByEntityWithLinkHeaderRelative()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("[]", Encoding.UTF8, JsonMime),
                 Headers =
                 {
                     {"Link", "<children/{id}>; rel=child; templated=true"}
                 }
             });
        await _endpoint.ReadAllAsync();

        _endpoint[new MockEntity(1, "test")]
           .Uri.Should().Be(new Uri("http://localhost/children/1"));
    }

    [Fact]
    public async Task TestGetByEntityWithLinkHeaderAbsolute()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("[]", Encoding.UTF8, JsonMime),
                 Headers =
                 {
                     {"Link", "<http://localhost/children/{id}>; rel=child; templated=true"}
                 }
             });
        await _endpoint.ReadAllAsync();

        _endpoint[new MockEntity(1, "test")]
           .Uri.Should().Be(new Uri("http://localhost/children/1"));
    }

    [Fact]
    public async Task TestReadAll()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(JsonMime, """[{"id":5,"name":"test1"},{"id":6,"name":"test2"}]""");

        var result = await _endpoint.ReadAllAsync();
        result.Should().Equal(new MockEntity(5, "test1"), new MockEntity(6, "test2"));
    }

    [Fact]
    public async Task TestReadAllCache()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""[{"id":5,"name":"test1"},{"id":6,"name":"test2"}]""", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"123abc\"")}
             });
        var result1 = await _endpoint.ReadAllAsync();
        result1.Should().Equal(new MockEntity(5, "test1"), new MockEntity(6, "test2"));

        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("If-None-Match", "\"123abc\"")
            .Respond(HttpStatusCode.NotModified);
        var result2 = await _endpoint.ReadAllAsync();
        result2.Should().Equal(new MockEntity(5, "test1"), new MockEntity(6, "test2"));

        result2.Should().NotBeSameAs(result1,
            because: "Cache responses, not deserialized objects");
    }

    [Fact]
    public async Task TestReadRangeOffset()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("Range", "elements=1-")
            .Respond(HttpStatusCode.PartialContent,
                 new StringContent("""[{"id":6,"name":"test2"}]""", Encoding.UTF8, JsonMime)
                 {
                     Headers = {ContentRange = new(from: 1, to: 1, length: 2) {Unit = "elements"}}
                 });

        var response = await _endpoint.ReadRangeAsync(new(from: 1, to: null));
        response.Elements.Should().Equal(new MockEntity {Id = 6, Name = "test2"});
        response.Range.Should().Be(new ContentRangeHeaderValue(from: 1, to: 1, length: 2) {Unit = "elements"});
    }

    [Fact]
    public async Task TestReadRangeHead()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("Range", "elements=0-1")
            .Respond(HttpStatusCode.PartialContent,
                 new StringContent("""[{"id":5,"name":"test1"},{"id":6,"name":"test2"}]""", Encoding.UTF8, JsonMime)
                 {
                     Headers = {ContentRange = new(from: 0, to: 1, length: 2) {Unit = "elements"}}
                 });

        var response = await _endpoint.ReadRangeAsync(new(from: 0, to: 1));
        response.Elements.Should().Equal(new MockEntity(5, "test1"), new MockEntity(6, "test2"));
        response.Range.Should().Be(new ContentRangeHeaderValue(from: 0, to: 1, length: 2) {Unit = "elements"});
    }

    [Fact]
    public async Task TestReadRangeTail()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("Range", "elements=-1")
            .Respond(HttpStatusCode.PartialContent,
                 new StringContent("""[{"id":6,"name":"test2"}]""", Encoding.UTF8, JsonMime)
                 {
                     Headers = {ContentRange = new(from: 2, to: 2) {Unit = "elements"}}
                 });

        var response = await _endpoint.ReadRangeAsync(new(from: null, to: 1));
        response.Elements.Should().Equal(new MockEntity(6, "test2"));
        response.Range.Should().Be(new ContentRangeHeaderValue(from: 2, to: 2) {Unit = "elements"});
    }

    [Fact]
    public async Task TestReadRangeException()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("Range", "elements=5-10")
            .Respond(HttpStatusCode.RequestedRangeNotSatisfiable, JsonMime, """{"message":"test"}""");

        string? exceptionMessage = null;
        try
        {
            await _endpoint.ReadRangeAsync(new RangeItemHeaderValue(from: 5, to: 10));
        }
        catch (InvalidOperationException ex)
        {
            exceptionMessage = ex.Message;
        }

        exceptionMessage.Should().Be("test");
    }

    [Fact]
    public async Task TestCreate()
    {
        Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
            .WithContent("""{"id":0,"name":"test"}""")
            .Respond(JsonMime, """{"id":5,"name":"test"}""");

        var element = (await _endpoint.CreateAsync(new MockEntity(0, "test")))!;
        element.Response.Should().Be(new MockEntity(5, "test"));
        element.Uri.Should().Be(new Uri("http://localhost/endpoint/5"));
    }

    [Fact]
    public async Task TestCreateLocation()
    {
        Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
            .WithContent("""{"id":0,"name":"test"}""")
            .Respond(_ => new(HttpStatusCode.Created)
             {
                 Content = new StringContent("""{"id":5,"name":"test"}""", Encoding.UTF8, JsonMime),
                 Headers = {Location = new Uri("/endpoint/new", UriKind.Relative)}
             });

        var element = (await _endpoint.CreateAsync(new MockEntity(0, "test")))!;
        element.Response.Should().Be(new MockEntity(5, "test"));
        element.Uri.Should().Be(new Uri("http://localhost/endpoint/new"));
    }

    [Fact]
    public async Task TestCreateNull()
    {
        Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
            .WithContent("""{"id":0,"name":"test"}""")
            .Respond(_ => new(HttpStatusCode.Accepted));

        var element = (await _endpoint.CreateAsync(new MockEntity(0, "test")))!;
        element.Should().BeNull();
    }

    [Fact]
    public async Task TestCreateAll()
    {
        Mock.Expect(HttpMethods.Patch, "http://localhost/endpoint")
            .WithContent("""[{"id":5,"name":"test1"},{"id":6,"name":"test2"}]""")
            .Respond(_ => new(HttpStatusCode.Accepted));

        await _endpoint.CreateAllAsync(new MockEntity[] {new(5, "test1"), new(6, "test2")});
    }

    [Fact]
    public async Task TestSetAll()
    {
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""[{"id":5,"name":"test1"},{"id":6,"name":"test2"}]""")
            .Respond(_ => new(HttpStatusCode.NoContent));

        await _endpoint.SetAllAsync(new MockEntity[] {new(5, "test1"), new(6, "test2")});
    }

    [Fact]
    public async Task TestSetAllETag()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""[{"id":5,"name":"test1"},{"id":6,"name":"test2"}]""", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"123abc\"")}
             });
        var result = await _endpoint.ReadAllAsync();

        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""[{"id":5,"name":"test1"},{"id":6,"name":"test2"}]""")
            .WithHeaders("If-Match", "\"123abc\"")
            .Respond(_ => new(HttpStatusCode.NoContent));

        await _endpoint.SetAllAsync(result);
    }
}
