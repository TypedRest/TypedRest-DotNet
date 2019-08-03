using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using RichardSzalay.MockHttp;
using Xunit;

namespace TypedRest.Endpoints.Rpc
{
    [Collection("Endpoint")]
    public class ActionEndpointTest : EndpointTestBase
    {
        private readonly IActionEndpoint _endpoint;

        public ActionEndpointTest()
        {
            _endpoint = new ActionEndpoint(EntryEndpoint, "endpoint");
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
