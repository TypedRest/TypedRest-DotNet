namespace TypedRest.Endpoints.Generic;

[Collection("Endpoint")]
public class ElementEndpointTest : EndpointTestBase
{
    private readonly ElementEndpoint<MockEntity> _endpoint;

    public ElementEndpointTest()
    {
        _endpoint = new(EntryEndpoint, "endpoint");
    }

    [Fact]
    public async Task TestRead()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(JsonMime, "{\"id\":5,\"name\":\"test\"}");

        var result = await _endpoint.ReadAsync();
        result.Should().Be(new MockEntity(5, "test"));
    }

    [Fact]
    public async Task TestReadCustomMimeWithJsonSuffix()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond("application/sample+json", "{\"id\":5,\"name\":\"test\"}");

        var result = await _endpoint.ReadAsync();
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
        var result1 = await _endpoint.ReadAsync();
        result1.Should().Be(new MockEntity(5, "test"));

        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("If-None-Match", "\"123abc\"")
            .Respond(HttpStatusCode.NotModified);
        var result2 = await _endpoint.ReadAsync();
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
        var result1 = await _endpoint.ReadAsync();
        result1.Should().Be(new MockEntity(5, "test"));

        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("If-Modified-Since", "Wed, 21 Oct 2015 00:00:00 GMT")
            .Respond(HttpStatusCode.NotModified);
        var result2 = await _endpoint.ReadAsync();
        result2.Should().Be(new MockEntity(5, "test"));

        result2.Should().NotBeSameAs(result1,
            because: "Cache responses, not deserialized objects");
    }

    [Fact]
    public async Task TestExistsTrue()
    {
        Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
            .Respond(HttpStatusCode.OK);

        bool result = await _endpoint.ExistsAsync();
        result.Should().BeTrue();
    }

    [Fact]
    public async Task TestExistsFalse()
    {
        Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
            .Respond(HttpStatusCode.NotFound);

        bool result = await _endpoint.ExistsAsync();
        result.Should().BeFalse();
    }

    [Fact]
    public async Task TestSetResult()
    {
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"test\"}")
            .Respond(JsonMime, "{\"id\":5,\"name\":\"testXXX\"}");

        var result = await _endpoint.SetAsync(new MockEntity(5, "test"));
        result.Should().Be(new MockEntity(5, "testXXX"));
    }

    [Fact]
    public async Task TestSetNoResult()
    {
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"test\"}")
            .Respond(HttpStatusCode.NoContent);

        var result = await _endpoint.SetAsync(new MockEntity(5, "test"));
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
        var result = await _endpoint.ReadAsync();

        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"test\"}")
            .WithHeaders("If-Match", "\"123abc\"")
            .Respond(HttpStatusCode.NoContent);
        await _endpoint.SetAsync(result);
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
        var result = await _endpoint.ReadAsync();

        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"test\"}")
            .WithHeaders("If-Unmodified-Since", "Wed, 21 Oct 2015 00:00:00 GMT")
            .Respond(HttpStatusCode.NoContent);
        await _endpoint.SetAsync(result);
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

        await _endpoint.UpdateAsync(x => x.Name = "testX");
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

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _endpoint.UpdateAsync(x => x.Name = "testX", maxRetries: 0));
    }

    [Fact]
    public async Task TestJsonPatch()
    {
        Mock.Expect(HttpMethods.Patch, "http://localhost/endpoint")
            .WithContent("[{\"value\":\"testX\",\"path\":\"/name\",\"op\":\"replace\"}]")
            .Respond(JsonMime, "{\"id\":5,\"name\":\"testX\"}");

        var result = await _endpoint.UpdateAsync(patch => patch.Replace(x => x.Name, "testX"));
        result.Should().Be(new MockEntity(5, "testX"));
    }

    [Fact]
    public async Task TestJsonPatchFallback()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("{\"id\":5,\"name\":\"test1\"}", Encoding.UTF8, JsonMime)
             });
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"testX\"}")
            .Respond(JsonMime, "{\"id\":5,\"name\":\"testX\"}");

        var result = await _endpoint.UpdateAsync(patch => patch.Replace(x => x.Name, "testX"));
        result.Should().Be(new MockEntity(5, "testX"));
    }

    [Fact]
    public async Task TestMergeResult()
    {
        Mock.Expect(HttpMethods.Patch, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"test\"}")
            .Respond(JsonMime, "{\"id\":5,\"name\":\"testXXX\"}");

        var result = await _endpoint.MergeAsync(new MockEntity(5, "test"));
        result.Should().Be(new MockEntity(5, "testXXX"));
    }

    [Fact]
    public async Task TestMergeNoResult()
    {
        Mock.Expect(HttpMethods.Patch, "http://localhost/endpoint")
            .WithContent("{\"id\":5,\"name\":\"test\"}")
            .Respond(HttpStatusCode.NoContent);

        var result = await _endpoint.MergeAsync(new MockEntity(5, "test"));
        result.Should().BeNull();
    }

    [Fact]
    public async Task TestDelete()
    {
        Mock.Expect(HttpMethod.Delete, "http://localhost/endpoint")
            .Respond(HttpStatusCode.NoContent);

        await _endpoint.DeleteAsync();
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
        await _endpoint.ReadAsync();

        Mock.Expect(HttpMethod.Delete, "http://localhost/endpoint")
            .WithHeaders("If-Match", "\"123abc\"")
            .Respond(HttpStatusCode.NoContent);

        await _endpoint.DeleteAsync();
    }
}