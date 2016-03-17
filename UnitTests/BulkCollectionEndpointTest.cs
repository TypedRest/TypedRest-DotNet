using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    [TestFixture]
    public class BulkCollectionEndpointTest : EndpointTestBase
    {
        private IBulkCollectionEndpoint<MockEntity> _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new BulkCollectionEndpoint<MockEntity>(EntryEndpoint, "endpoint");
        }

        [Test]
        public async Task TestSetAll()
        {
            Mock.Expect(HttpMethod.Put, "http://localhost/endpoint/")
                .WithContent("[{\"Id\":5,\"Name\":\"test1\"},{\"Id\":6,\"Name\":\"test2\"}]")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent));

            await _endpoint.SetAllAsync(new[] { new MockEntity(5, "test1"), new MockEntity(6, "test2") });
        }

        [Test]
        public async Task TestCreate()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint/")
                .WithContent("[{\"Id\":5,\"Name\":\"test1\"},{\"Id\":6,\"Name\":\"test2\"}]")
                .Respond(new HttpResponseMessage(HttpStatusCode.Accepted));

            await _endpoint.CreateAsync(new[] { new MockEntity(5, "test1"), new MockEntity(6, "test2") });
        }
    }
}