using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using RichardSzalay.MockHttp;
using Xunit;

namespace TypedRest
{
    public class ActionEndpointTest : EndpointTestBase
    {
        private readonly IActionEndpoint _endpoint;

        public ActionEndpointTest() => _endpoint = new ActionEndpoint(EntryEndpoint, "endpoint");

        [Fact]
        public async Task TestProbe()
        {
            Mock.Expect(HttpMethod.Options, "http://localhost/endpoint")
                .Respond(new StringContent("") {Headers = {Allow = {"POST"}}});

            await _endpoint.ProbeAsync();

            _endpoint.TriggerAllowed.Should().BeTrue();
        }

        [Fact]
        public async Task TestTrigger()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .Respond(HttpStatusCode.Accepted);

            await _endpoint.TriggerAsync();
        }
    }
}