namespace TypedRest.Endpoints.Reactive;

[Collection("Endpoint")]
public class SseStreamingEndpointTest : EndpointTestBase
{
    private readonly SseStreamingEndpoint<MockEntity> _endpoint;

    public SseStreamingEndpointTest()
    {
        _endpoint = new(EntryEndpoint, "endpoint") {AutoReconnect = false};
    }

    [Fact]
    public void TestGetObservable()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond("text/event-stream",
                "data: {\"id\":5,\"name\":\"test1\"}\n\n" +
                "data: {\"id\":6,\"name\":\"test2\"}\n\n" +
                "data: {\"id\":7,\"name\":\"test3\"}\n\n");

        _endpoint.GetObservable().ToEnumerable().ToList().Should().Equal(
            new MockEntity(5, "test1"),
            new MockEntity(6, "test2"),
            new MockEntity(7, "test3"));
    }

    [Fact]
    public void TestEventTypeFilter()
    {
        var endpoint = new SseStreamingEndpoint<MockEntity>(EntryEndpoint, "endpoint", eventType: "update") {AutoReconnect = false};

        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond("text/event-stream",
                "event: ignored\ndata: {\"id\":1,\"name\":\"skip\"}\n\n" +
                "event: update\ndata: {\"id\":5,\"name\":\"test1\"}\n\n" +
                "data: {\"id\":99,\"name\":\"default\"}\n\n" +
                "event: update\ndata: {\"id\":6,\"name\":\"test2\"}\n\n");

        endpoint.GetObservable().ToEnumerable().ToList().Should().Equal(
            new MockEntity(5, "test1"),
            new MockEntity(6, "test2"));
    }

    [Fact]
    public void TestErrorHandling()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.Conflict)
             {
                 Content = new StringContent("""{"message":"my message"}""")
             });

        _endpoint.Invoking(x =>
        {
            _ = x.GetObservable().ToEnumerable().ToList();
        }).Should().Throw<InvalidOperationException>("my message");
    }

    [Fact]
    public void TestNoContentCompletes()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(HttpStatusCode.NoContent);

        _endpoint.AutoReconnect = true;
        _endpoint.GetObservable().ToEnumerable().ToList().Should().BeEmpty();
    }
}
