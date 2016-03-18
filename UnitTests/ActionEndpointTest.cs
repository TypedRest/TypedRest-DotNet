using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    [TestFixture]
    public class ActionEndpointTest : EndpointTestBase
    {
        private IActionEndpoint _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new ActionEndpoint(EntryEndpoint, "endpoint");
        }

        [Test]
        public async Task TestProbe()
        {
            Mock.Expect(HttpMethod.Options, "http://localhost/endpoint")
                .Respond(new StringContent("") {Headers = {Allow = {"POST"}}});

            await _endpoint.ProbeAsync();

            _endpoint.TriggerAllowed.Should().BeTrue();
        }

        [Test]
        public async Task TestTrigger()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .Respond(HttpStatusCode.NoContent);

            await _endpoint.TriggerAsync();
        }

        [Test]
        public async Task TestTriggerWithBody()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .Respond(HttpStatusCode.Accepted);

            await _endpoint.TriggerAsync(new MockEntity(4711, "mock"));
        }

        [Test]
        public async Task TestTriggerWithBodyAndResult()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .Respond(TriggerHandler);

            var entity = new MockEntity(4711, "mock");
            long id = await _endpoint.TriggerAsync<MockEntity,long>(entity);

            id.Should().Be(entity.Id);
        }

        [Test]
        public async Task TestTriggerWithResult()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .Respond(TriggerHandler);

            long id = await _endpoint.TriggerAsync<long>();

            id.Should().Be(1234);
        }

        private static async Task<HttpResponseMessage> TriggerHandler(HttpRequestMessage httpRequestMessage)
        {
            long result = 1234;

            if (httpRequestMessage.Content != null)
            {
                var content = await httpRequestMessage.Content.ReadAsAsync<MockEntity>();
                result = content.Id;
            }

            httpRequestMessage.SetConfiguration(new HttpConfiguration());
            return httpRequestMessage.CreateResponse(HttpStatusCode.Accepted, result);
        }
    }
}