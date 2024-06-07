namespace TypedRest.Endpoints.Rpc;

[Collection("Endpoint")]
public class ConsumerEndpointTest : EndpointTestBase
{
    private readonly ConsumerEndpoint<MockEntity> _endpoint;

    public ConsumerEndpointTest()
    {
        _endpoint = new(EntryEndpoint, "endpoint");
    }

    [Fact]
    public async Task TestInvoke()
    {
        Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
            .WithContent("""{"id":1,"name":"input"}""")
            .Respond(HttpStatusCode.Accepted);

        await _endpoint.InvokeAsync(new MockEntity(1, "input"));
    }
}
