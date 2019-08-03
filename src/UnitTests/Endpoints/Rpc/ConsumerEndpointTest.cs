using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using Xunit;

namespace TypedRest.Endpoints.Rpc
{
    [Collection("Endpoint")]
    public class ConsumerEndpointTest : EndpointTestBase
    {
        private readonly IConsumerEndpoint<MockEntity> _endpoint;

        public ConsumerEndpointTest()
        {
            _endpoint = new ConsumerEndpoint<MockEntity>(EntryEndpoint, "endpoint");
        }

        [Fact]
        public async Task TestInvoke()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .WithContent("{\"id\":1,\"name\":\"input\"}")
                .Respond(HttpStatusCode.Accepted);

            await _endpoint.InvokeAsync(new MockEntity(1, "input"));
        }
    }
}
