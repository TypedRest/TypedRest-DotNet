namespace TypedRest.Endpoints.Rpc
{
    [Collection("Endpoint")]
    public class ActionEndpointTest : EndpointTestBase
    {
        private readonly ActionEndpoint _endpoint;

        public ActionEndpointTest()
        {
            _endpoint = new(EntryEndpoint, "endpoint");
        }

        [Fact]
        public async Task TestProbe()
        {
            Mock.Expect(HttpMethod.Options, "http://localhost/endpoint")
                .Respond(_ => new StringContent("") {Headers = {Allow = {"POST"}}});

            await _endpoint.ProbeAsync();

            _endpoint.InvokeAllowed.Should().BeTrue();
        }

        [Fact]
        public async Task TestInvoke()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .Respond(HttpStatusCode.Accepted);

            await _endpoint.InvokeAsync();
        }
    }
}
