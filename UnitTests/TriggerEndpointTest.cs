using System.Threading.Tasks;
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
        public async Task TestTrigger()
        {
            //stubFor(post(urlEqualTo("/endpoint"))
            //    .withHeader("Accept", equalTo(jsonMime))
            //    .willReturn(aResponse().withStatus(SC_OK)));

            await _endpoint.TriggerAsync();
        }
    }
}