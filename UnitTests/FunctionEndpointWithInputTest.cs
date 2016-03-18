using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    public class FunctionEndpointWithInputTest : EndpointTestBase
    {
        private IFunctionEndpoint<MockEntity, MockEntity> _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new FunctionEndpoint<MockEntity, MockEntity>(EntryEndpoint, "endpoint");
        }

        [Test]
        public async Task TestTrigger()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .WithContent("{\"Id\":1,\"Name\":\"input\"}")
                .Respond(JsonMime, "{\"Id\":2,\"Name\":\"result\"}");

            var result = await _endpoint.TriggerAsync(new MockEntity(1, "input"));
            result.Should().Be(new MockEntity(2, "result"));
        }
    }
}