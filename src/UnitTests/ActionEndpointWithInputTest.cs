using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using Xunit;

namespace TypedRest
{
    [Collection("Endpoint")]
    public class ActionEndpointWithInputTest : EndpointTestBase
    {
        private readonly IActionEndpoint<MockEntity> _endpoint;

        public ActionEndpointWithInputTest()
        {
            _endpoint = new ActionEndpoint<MockEntity>(EntryEndpoint, "endpoint");
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
