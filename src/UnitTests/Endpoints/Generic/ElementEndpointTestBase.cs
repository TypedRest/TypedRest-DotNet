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
            .Respond(JsonMime, """{"id":5,"name":"test"}""");

        var result = await Endpoint.ReadAsync(TestContext.Current.CancellationToken);
        result.Should().Be(new MockEntity(5, "test"));
    }

    [Fact]
    public async Task TestReadCustomMimeWithJsonSuffix()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond("application/sample+json", """{"id":5,"name":"test"}""");

        var result = await Endpoint.ReadAsync(TestContext.Current.CancellationToken);
        result.Should().Be(new MockEntity(5, "test"));
    }

    [Fact]
    public async Task TestReadCacheETag()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":5,"name":"test"}""", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"123abc\"")}
             });
        var result1 = await Endpoint.ReadAsync(TestContext.Current.CancellationToken);
        result1.Should().Be(new MockEntity(5, "test"));

        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("If-None-Match", "\"123abc\"")
            .Respond(HttpStatusCode.NotModified);
        var result2 = await Endpoint.ReadAsync(TestContext.Current.CancellationToken);
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
                 Content = new StringContent("""{"id":5,"name":"test"}""", Encoding.UTF8, JsonMime)
                 {
                     Headers = {LastModified = new(new DateTime(2015, 10, 21), TimeSpan.Zero)}
                 }
             });
        var result1 = await Endpoint.ReadAsync(TestContext.Current.CancellationToken);
        result1.Should().Be(new MockEntity(5, "test"));

        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("If-Modified-Since", "Wed, 21 Oct 2015 00:00:00 GMT")
            .Respond(HttpStatusCode.NotModified);
        var result2 = await Endpoint.ReadAsync(TestContext.Current.CancellationToken);
        result2.Should().Be(new MockEntity(5, "test"));

        result2.Should().NotBeSameAs(result1,
            because: "Cache responses, not deserialized objects");
    }

    [Fact]
    public async Task TestReadCacheNoCache()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":5,"name":"test"}""", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"123abc\""), CacheControl = new() {NoCache = true}}
             });
        await Endpoint.ReadAsync(TestContext.Current.CancellationToken);

        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("If-None-Match", "\"123abc\"")
            .Respond(HttpStatusCode.NotModified);
        var result = await Endpoint.ReadAsync(TestContext.Current.CancellationToken);
        result.Should().Be(new MockEntity(5, "test"));
    }

    [Fact]
    public async Task TestReadCacheExpired()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":5,"name":"test"}""", Encoding.UTF8, JsonMime)
                 {
                     Headers = {Expires = new(new DateTime(2015, 10, 21), TimeSpan.Zero)}
                 },
                 Headers = {ETag = new("\"123abc\"")}
             });
        await Endpoint.ReadAsync(TestContext.Current.CancellationToken);

        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("If-None-Match", "\"123abc\"")
            .Respond(HttpStatusCode.NotModified);
        var result = await Endpoint.ReadAsync(TestContext.Current.CancellationToken);
        result.Should().Be(new MockEntity(5, "test"));
    }

    [Fact]
    public async Task TestExistsTrue()
    {
        Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
            .Respond(HttpStatusCode.OK);

        bool result = await Endpoint.ExistsAsync(TestContext.Current.CancellationToken);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task TestExistsFalse()
    {
        Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
            .Respond(HttpStatusCode.NotFound);

        bool result = await Endpoint.ExistsAsync(TestContext.Current.CancellationToken);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task TestSetResult()
    {
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"test"}""")
            .Respond(JsonMime, """{"id":5,"name":"testXXX"}""");

        var result = await Endpoint.SetAsync(new MockEntity(5, "test"), TestContext.Current.CancellationToken);
        result.Should().Be(new MockEntity(5, "testXXX"));
    }

    [Fact]
    public async Task TestSetNoResult()
    {
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"test"}""")
            .Respond(HttpStatusCode.NoContent);

        var result = await Endpoint.SetAsync(new MockEntity(5, "test"), TestContext.Current.CancellationToken);
        result.Should().BeNull();
    }

    [Fact]
    public async Task TestSetETag()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":5,"name":"test"}""", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"123abc\"")}
             });
        var result = await Endpoint.ReadAsync(TestContext.Current.CancellationToken);

        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"test"}""")
            .WithHeaders("If-Match", "\"123abc\"")
            .Respond(HttpStatusCode.NoContent);
        await Endpoint.SetAsync(result, TestContext.Current.CancellationToken);
    }

    /// <summary>
    /// Reads the entity with ETag <c>"v1"</c>, then starts another read whose response is held back until the returned function is called.
    /// </summary>
    private async Task<(Task<MockEntity> Read, Action Release)> StartDelayedReadAsync(Func<HttpResponseMessage> response)
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":5,"name":"test"}""", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"v1\"")}
             });
        await Endpoint.ReadAsync(TestContext.Current.CancellationToken);

        var requested = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var released = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("If-None-Match", "\"v1\"")
            .Respond(async _ =>
             {
                 requested.SetResult(true);
                 await released.Task;
                 return response();
             });
        var read = Endpoint.ReadAsync(TestContext.Current.CancellationToken);
        await requested.Task;
        return (read, () => released.SetResult(true));
    }

    [Fact]
    public async Task TestReadNotModifiedDuringSet()
    {
        var (read, release) = await StartDelayedReadAsync(() => new(HttpStatusCode.NotModified));

        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.NoContent) {Headers = {ETag = new("\"v2\"")}});
        await Endpoint.SetAsync(new MockEntity(5, "changed"), TestContext.Current.CancellationToken);
        release();

        (await read).Should().Be(new MockEntity(5, "test"),
            because: "The server confirmed the version the read was based on, even though the cache has moved on since");
        Endpoint.Response.Should().Be(new MockEntity(5, "changed"));
    }

    [Fact]
    public async Task TestReadOutdatedDuringSet()
    {
        var (read, release) = await StartDelayedReadAsync(() => new()
        {
            Content = new StringContent("""{"id":5,"name":"outdated"}""", Encoding.UTF8, JsonMime),
            Headers = {ETag = new("\"v1b\"")}
        });

        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.NoContent) {Headers = {ETag = new("\"v2\"")}});
        await Endpoint.SetAsync(new MockEntity(5, "changed"), TestContext.Current.CancellationToken);
        release();

        (await read).Should().Be(new MockEntity(5, "outdated"));
        Endpoint.Response.Should().Be(new MockEntity(5, "changed"),
            because: "The outdated response must not replace the version from the newer write");

        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithHeaders("If-Match", "\"v2\"")
            .Respond(HttpStatusCode.NoContent);
        await Endpoint.SetAsync(new MockEntity(5, "changed again"), TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task TestSetWeakETag()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":5,"name":"test"}""", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"v1\"", isWeak: true)}
             });
        await Endpoint.ReadAsync(TestContext.Current.CancellationToken);

        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .With(request => request.Headers.IfMatch.Count == 0) // If-Match uses strong comparison, so a weak ETag would never match
            .Respond(HttpStatusCode.NoContent);
        await Endpoint.SetAsync(new MockEntity(5, "changed"), TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task TestSetLastModified()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":5,"name":"test"}""", Encoding.UTF8, JsonMime)
                 {
                     Headers = {LastModified = new(new DateTime(2015, 10, 21), TimeSpan.Zero)}
                 }
             });
        var result = await Endpoint.ReadAsync(TestContext.Current.CancellationToken);

        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"test"}""")
            .WithHeaders("If-Unmodified-Since", "Wed, 21 Oct 2015 00:00:00 GMT")
            .Respond(HttpStatusCode.NoContent);
        await Endpoint.SetAsync(result, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task TestSetCacheETag()
    {
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"test"}""")
            .Respond(_ => new(HttpStatusCode.NoContent) {Headers = {ETag = new("\"456def\"")}});
        await Endpoint.SetAsync(new MockEntity(5, "test"), TestContext.Current.CancellationToken);

        Endpoint.Response.Should().Be(new MockEntity(5, "test"));

        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .WithHeaders("If-None-Match", "\"456def\"")
            .Respond(HttpStatusCode.NotModified);
        var result = await Endpoint.ReadAsync(TestContext.Current.CancellationToken);
        result.Should().Be(new MockEntity(5, "test"));

        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"test"}""")
            .WithHeaders("If-Match", "\"456def\"")
            .Respond(HttpStatusCode.NoContent);
        await Endpoint.SetAsync(result, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task TestSetCacheETagWithResult()
    {
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"test"}""")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":5,"name":"testXXX"}""", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"456def\"")}
             });
        var result = await Endpoint.SetAsync(new MockEntity(5, "test"), TestContext.Current.CancellationToken);
        result.Should().Be(new MockEntity(5, "testXXX"));

        Endpoint.Response.Should().Be(new MockEntity(5, "testXXX"));

        Mock.Expect(HttpMethod.Delete, "http://localhost/endpoint")
            .WithHeaders("If-Match", "\"456def\"")
            .Respond(HttpStatusCode.NoContent);
        await Endpoint.DeleteAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task TestSetNoETag()
    {
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"test"}""")
            .Respond(HttpStatusCode.NoContent);
        await Endpoint.SetAsync(new MockEntity(5, "test"), TestContext.Current.CancellationToken);

        Endpoint.Response.Should().BeNull();
    }

    [Fact]
    public async Task TestUpdateRetry()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":5,"name":"test1"}""", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"1\"")}
             });
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"testX"}""")
            .WithHeaders("If-Match", "\"1\"")
            .Respond(HttpStatusCode.PreconditionFailed);
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":5,"name":"test2"}""", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"2\"")}
             });
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"testX"}""")
            .WithHeaders("If-Match", "\"2\"")
            .Respond(HttpStatusCode.NoContent);

        await Endpoint.UpdateAsync(x => x.Name = "testX", cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task TestUpdateFail()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":5,"name":"test1"}""", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"1\"")}
             });
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"testX"}""")
            .WithHeaders("If-Match", "\"1\"")
            .Respond(HttpStatusCode.PreconditionFailed);

        await Assert.ThrowsAsync<InvalidOperationException>(() => Endpoint.UpdateAsync(x => x.Name = "testX", maxRetries: 0, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task TestMergeResult()
    {
        Mock.Expect(HttpMethods.Patch, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"test"}""")
            .Respond(JsonMime, """{"id":5,"name":"testXXX"}""");

        var result = await Endpoint.MergeAsync(new MockEntity(5, "test"), TestContext.Current.CancellationToken);
        result.Should().Be(new MockEntity(5, "testXXX"));
    }

    [Fact]
    public async Task TestMergeNoResult()
    {
        Mock.Expect(HttpMethods.Patch, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"test"}""")
            .Respond(HttpStatusCode.NoContent);

        var result = await Endpoint.MergeAsync(new MockEntity(5, "test"), TestContext.Current.CancellationToken);
        result.Should().BeNull();
    }

    [Fact]
    public async Task TestDelete()
    {
        Mock.Expect(HttpMethod.Delete, "http://localhost/endpoint")
            .Respond(HttpStatusCode.NoContent);

        await Endpoint.DeleteAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task TestDeleteETag()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":5,"name":"test"}""", Encoding.UTF8, JsonMime),
                 Headers = {ETag = new("\"123abc\"")}
             });
        await Endpoint.ReadAsync(TestContext.Current.CancellationToken);

        Mock.Expect(HttpMethod.Delete, "http://localhost/endpoint")
            .WithHeaders("If-Match", "\"123abc\"")
            .Respond(HttpStatusCode.NoContent);

        await Endpoint.DeleteAsync(TestContext.Current.CancellationToken);
    }
}
