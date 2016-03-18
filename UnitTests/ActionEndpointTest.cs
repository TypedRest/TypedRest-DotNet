using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
                .Respond(HttpStatusCode.Accepted);

            await _endpoint.TriggerAsync();
        }
    }
}