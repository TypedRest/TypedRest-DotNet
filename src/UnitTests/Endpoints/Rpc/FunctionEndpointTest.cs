namespace TypedRest.Endpoints.Rpc
{
    [Collection("Endpoint")]
    public class FunctionEndpointTest : EndpointTestBase
    {
        private readonly FunctionEndpoint<MockEntity, MockEntity> _endpoint;

        public FunctionEndpointTest()
        {
            _endpoint = new(EntryEndpoint, "endpoint");
        }

        [Fact]
        public async Task TestInvoke()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .WithContent("{\"id\":1,\"name\":\"input\"}")
                .Respond(JsonMime, "{\"id\":2,\"name\":\"result\"}");

            var result = await _endpoint.InvokeAsync(new MockEntity(1, "input"));
            result.Should().Be(new MockEntity(2, "result"));
        }
    }
}
