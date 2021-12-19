namespace TypedRest.Endpoints.Rpc
{
    [Collection("Endpoint")]
    public class ProducerEndpointTest : EndpointTestBase
    {
        private readonly ProducerEndpoint<MockEntity> _endpoint;

        public ProducerEndpointTest()
        {
            _endpoint = new(EntryEndpoint, "endpoint");
        }

        [Fact]
        public async Task TestInvoke()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .Respond(JsonMime, "{\"id\":2,\"name\":\"result\"}");

            var result = await _endpoint.InvokeAsync();
            result.Should().Be(new MockEntity(2, "result"));
        }
    }
}
