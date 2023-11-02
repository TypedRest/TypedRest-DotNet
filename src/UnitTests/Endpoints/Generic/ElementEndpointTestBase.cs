namespace TypedRest.Endpoints.Generic;

public abstract class ElementEndpointTestBase : EndpointTestBase
{
    protected readonly ElementEndpoint<MockEntity> Endpoint;

    protected ElementEndpointTestBase(MediaTypeFormatter serializer) : base(serializer)
    {
        Endpoint = new(EntryEndpoint, "endpoint");
    }

    [Fact]
    public async Task TestRead()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(JsonMime, "{\"id\":5,\"name\":\"test\"}");

        var result = await Endpoint.ReadAsync();
        result.Should().Be(new MockEntity(5, "test"));
    }

    [Fact]
    public async Task TestReadCustomMimeWithJsonSuffix()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond("application/sample+json", "{\"id\":5,\"name\":\"test\"}");

        var result = await Endpoint.ReadAsync();
        result.Should().Be(new MockEntity(5, "test"));
    }

    [Fact]
    public async Task TestReadCacheETag()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("{\"id\":5,\"name\":\"test\"}", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"123abc\"")}
             });
        var result1 = await Endpoint.ReadAsync();
        result1.Should().Be(new MockEntity(5, "test"));

        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("If-None-Match", "\"123abc\"")
            .Respond(HttpStatusCode.NotModified);
        var result2 = await Endpoint.ReadAsync();
        result2.Should().Be(new MockEntity(5, "test"));

        result2.Should().NotBeSameAs(result1,
            because: "Cache responses, not deserialized objects");
    }

    [Fact]
    public async Task TestReadCacheLastModified()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("{\"id\":5,\"name\":\"test\"}", Encoding.UTF8, JsonMime)
                 {
                     Headers = {LastModified = new(new DateTime(2015, 10, 21), TimeSpan.Zero)}
                 }
             });
        var result1 = await Endpoint.ReadAsync();
        result1.Should().Be(new MockEntity(5, "test"));

        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("If-Modified-Since", "Wed, 21 Oct 2015 00:00:00 GMT")
            .Respond(HttpStatusCode.NotModified);
        var result2 = await Endpoint.ReadAsync();
        result2.Should().Be(new MockEntity(5, "test"));

        result2.Should().NotBeSameAs(result1,
            because: "Cache responses, not deserialized objects");
    }

    [Fact]
    public async Task TestExistsTrue()
    {
        Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
            .Respond(HttpStatusCode.OK);

        bool result = await Endpoint.ExistsAsync();
        result.Should().BeTrue();
    }

    [Fact]
    public async Task TestExistsFalse()
    {
        Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
            .Respond(HttpStatusCode.NotFound);

        bool result = await Endpoint.ExistsAsync();
        result.Should().BeFalse();
    }

    [Fact]
    public async Task TestSetResult()
    {
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"test\"}")
            .Respond(JsonMime, "{\"id\":5,\"name\":\"testXXX\"}");

        var result = await Endpoint.SetAsync(new MockEntity(5, "test"));
        result.Should().Be(new MockEntity(5, "testXXX"));
    }

    [Fact]
    public async Task TestSetNoResult()
    {
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"test\"}")
            .Respond(HttpStatusCode.NoContent);

        var result = await Endpoint.SetAsync(new MockEntity(5, "test"));
        result.Should().BeNull();
    }

    [Fact]
    public async Task TestSetETag()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("{\"id\":5,\"name\":\"test\"}", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"123abc\"")}
             });
        var result = await Endpoint.ReadAsync();

        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"test\"}")
            .WithHeaders("If-Match", "\"123abc\"")
            .Respond(HttpStatusCode.NoContent);
        await Endpoint.SetAsync(result);
    }

    [Fact]
    public async Task TestSetLastModified()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("{\"id\":5,\"name\":\"test\"}", Encoding.UTF8, JsonMime)
                 {
                     Headers = {LastModified = new(new DateTime(2015, 10, 21), TimeSpan.Zero)}
                 }
             });
        var result = await Endpoint.ReadAsync();

        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"test\"}")
            .WithHeaders("If-Unmodified-Since", "Wed, 21 Oct 2015 00:00:00 GMT")
            .Respond(HttpStatusCode.NoContent);
        await Endpoint.SetAsync(result);
    }

    [Fact]
    public async Task TestUpdateRetry()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("{\"id\":5,\"name\":\"test1\"}", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"1\"")}
             });
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"testX\"}")
            .WithHeaders("If-Match", "\"1\"")
            .Respond(HttpStatusCode.PreconditionFailed);
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("{\"id\":5,\"name\":\"test2\"}", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"2\"")}
             });
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"testX\"}")
            .WithHeaders("If-Match", "\"2\"")
            .Respond(HttpStatusCode.NoContent);

        await Endpoint.UpdateAsync(x => x.Name = "testX");
    }

    [Fact]
    public async Task TestUpdateFail()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("{\"id\":5,\"name\":\"test1\"}", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"1\"")}
             });
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"testX\"}")
            .WithHeaders("If-Match", "\"1\"")
            .Respond(HttpStatusCode.PreconditionFailed);

        await Assert.ThrowsAsync<InvalidOperationException>(() => Endpoint.UpdateAsync(x => x.Name = "testX", maxRetries: 0));
    }

    [Fact]
    public async Task TestMergeResult()
    {
        Mock.Expect(HttpMethods.Patch, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"test\"}")
            .Respond(JsonMime, "{\"id\":5,\"name\":\"testXXX\"}");

        var result = await Endpoint.MergeAsync(new MockEntity(5, "test"));
        result.Should().Be(new MockEntity(5, "testXXX"));
    }

    [Fact]
    public async Task TestMergeNoResult()
    {
        Mock.Expect(HttpMethods.Patch, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"test\"}")
            .Respond(HttpStatusCode.NoContent);

        var result = await Endpoint.MergeAsync(new MockEntity(5, "test"));
        result.Should().BeNull();
    }

    [Fact]
    public async Task TestDelete()
    {
        Mock.Expect(HttpMethod.Delete, "http://localhost/endpoint")
            .Respond(HttpStatusCode.NoContent);

        await Endpoint.DeleteAsync();
    }

    [Fact]
    public async Task TestDeleteETag()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("{\"id\":5,\"name\":\"test\"}", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"123abc\"")}
             });
        await Endpoint.ReadAsync();

        Mock.Expect(HttpMethod.Delete, "http://localhost/endpoint")
            .WithHeaders("If-Match", "\"123abc\"")
            .Respond(HttpStatusCode.NoContent);

        await Endpoint.DeleteAsync();
    }
}
