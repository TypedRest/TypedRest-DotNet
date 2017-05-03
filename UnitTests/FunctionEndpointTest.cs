using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using RichardSzalay.MockHttp;
using Xunit;

namespace TypedRest
{
    public class FunctionEndpointTest : EndpointTestBase
    {
        private readonly IFunctionEndpoint<MockEntity> _endpoint;

        public FunctionEndpointTest() => _endpoint = new FunctionEndpoint<MockEntity>(EntryEndpoint, "endpoint");

        [Fact]
        public async Task TestTrigger()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .Respond(JsonMime, "{\"Id\":2,\"Name\":\"result\"}");

            var result = await _endpoint.TriggerAsync();
            result.Should().Be(new MockEntity(2, "result"));
        }
    }
}