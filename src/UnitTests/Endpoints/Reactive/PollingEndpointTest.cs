namespace TypedRest.Endpoints.Reactive;

[Collection("Endpoint")]
public class PollingEndpointTest : EndpointTestBase
{
    private readonly PollingEndpoint<MockEntity> _endpoint;

    public PollingEndpointTest()
    {
        _endpoint = new(EntryEndpoint, "endpoint", endCondition: x => x.Id == 3)
        {
            PollingInterval = TimeSpan.Zero
        };
    }

    [Fact]
    public void TestGetObservable()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(JsonMime, """{"id":1,"name":"test"}""");
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(JsonMime, """{"id":2,"name":"test"}""");
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":3,"name":"test"}""", Encoding.UTF8, JsonMime),
                 Headers = {RetryAfter = new(TimeSpan.FromSeconds(42))}
             });

        var observable = _endpoint.GetObservable();
        observable.ToEnumerable().ToList().Should().Equal(
            new MockEntity(1, "test"),
            new MockEntity(2, "test"),
            new MockEntity(3, "test"));
        _endpoint.PollingInterval.Should().Be(TimeSpan.FromSeconds(42));
    }
}
