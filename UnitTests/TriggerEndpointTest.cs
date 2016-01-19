using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace TypedRest
{
    [TestFixture, Ignore("Server mock not implemented yet")]
    public class TriggerEndpointTest : EndpointTestBase
    {
        private TriggerEndpoint _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new TriggerEndpoint(EntryEndpoint, "endpoint");
        }

        [Test]
        public async Task TestProbe()
        {
            //stubFor(options(urlEqualTo("/endpoint"))
            //    .willReturn(aResponse()
            //        .withStatus(SC_OK)
            //        .withHeader("Allow", "POST")));

            await _endpoint.ProbeAsync();

            _endpoint.TriggerAllowed.Should().BeTrue();
        }

        [Test]
        public async Task TestTrigger()
        {
            //stubFor(post(urlEqualTo("/endpoint"))
            //    .willReturn(aResponse().withStatus(SC_OK)));

            await _endpoint.TriggerAsync();
        }
    }
}