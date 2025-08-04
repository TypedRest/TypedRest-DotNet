namespace TypedRest.Endpoints.Reactive;

[Collection("Endpoint")]
public class StreamingEndpointTest : EndpointTestBase
{
    private readonly StreamingEndpoint<MockEntity> _endpoint;

    public StreamingEndpointTest()
    {
        _endpoint = new(EntryEndpoint, "endpoint");
    }

    [Fact]
    public void TestGetObservable()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(JsonMime, """
                               {"id":5,"name":"test1"}
                               {"id":6,"name":"test2"}
                               {"id":7,"name":"test3"}
                               """);

        var observable = _endpoint.GetObservable();
        observable.ToEnumerable().ToList().Should().Equal(
            new MockEntity(5, "test1"),
            new MockEntity(6, "test2"),
            new MockEntity(7, "test3"));
    }

    [Fact]
    public void TestErrorHandling()
    {
        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new(HttpStatusCode.Conflict)
             {
                 Content = new StringContent("""{"message":"my message"}""")
             });

        var observable = _endpoint.GetObservable();
        observable.Invoking(x =>
        {
            _ = x.ToEnumerable().ToList();
        }).Should().Throw<InvalidOperationException>("my message");
    }
}
