using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    public class FunctionEndpointTest : EndpointTestBase
    {
        private IFunctionEndpoint<MockEntity> _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new FunctionEndpoint<MockEntity>(EntryEndpoint, "endpoint");
        }

        [Test]
        public async Task TestTrigger()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .Respond(JsonMime, "{\"Id\":2,\"Name\":\"result\"}");

            var result = await _endpoint.TriggerAsync();
            result.Should().Be(new MockEntity(2, "result"));
        }
    }
}