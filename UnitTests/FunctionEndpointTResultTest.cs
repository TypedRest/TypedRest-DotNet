using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    public class FunctionEndpointTResultTest : EndpointTestBase
    {
        private IFunctionEndpoint<int> _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new FunctionEndpoint<int>(EntryEndpoint, "endpoint");
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
        public async Task TestFunction()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .Respond(request =>
                {
                    request.SetConfiguration(new HttpConfiguration());
                    return request.CreateResponse(HttpStatusCode.Accepted, 4711);
                });

            int result = await _endpoint.TriggerAsync();

            result.Should().Be(4711);
        }
    }
}