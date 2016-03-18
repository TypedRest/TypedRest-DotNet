using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    public class ActionEndpointWithInputTest : EndpointTestBase
    {
        private IActionEndpoint<MockEntity> _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new ActionEndpoint<MockEntity>(EntryEndpoint, "endpoint");
        }

        [Test]
        public async Task TestTrigger()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .WithContent("{\"Id\":1,\"Name\":\"input\"}")
                .Respond(HttpStatusCode.Accepted);

            await _endpoint.TriggerAsync(new MockEntity(1, "input"));
        }
    }
}