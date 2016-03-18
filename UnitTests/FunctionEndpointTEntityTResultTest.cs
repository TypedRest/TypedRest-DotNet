using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    public class FunctionEndpointTEntityTResultTest : EndpointTestBase
    {
        private IFunctionEndpoint<MockEntity, int> _endpoint;
        private MockEntity _transmittedEntity;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new FunctionEndpoint<MockEntity, int>(EntryEndpoint, "endpoint");
        }

        [Test]
        public async Task TestProbe()
        {
            Mock.Expect(HttpMethod.Options, "http://localhost/endpoint")
                .Respond(new StringContent("") { Headers = { Allow = { "POST" } } });

            await _endpoint.ProbeAsync();

            _endpoint.TriggerAllowed.Should().BeTrue();
        }

        [Test]
        public async Task TestTrigger()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .Respond(Handler);

            var mockEntity = new MockEntity(4711, "mock");
            var result = await _endpoint.TriggerAsync(mockEntity);

            mockEntity.Should().Be(_transmittedEntity);
            result.Should().Be(4712);
        }

        private async Task<HttpResponseMessage> Handler(HttpRequestMessage httpRequestMessage)
        {
            // Have to create a new object, otherwise it's same object reference
            MockEntity requestEntity = await httpRequestMessage.Content.ReadAsAsync<MockEntity>();
            _transmittedEntity = new MockEntity(requestEntity.Id, requestEntity.Name);

            httpRequestMessage.SetConfiguration(new HttpConfiguration());
            return httpRequestMessage.CreateResponse(HttpStatusCode.Accepted, 4712);
        }
    }
}