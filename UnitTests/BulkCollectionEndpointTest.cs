using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

            await _endpoint.SetAllAsync(new[] {new MockEntity(5, "test1"), new MockEntity(6, "test2")});
        }

        [Test]
        public async Task TestSetAllETag()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint/")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("[{\"Id\":5,\"Name\":\"test1\"},{\"Id\":6,\"Name\":\"test2\"}]", Encoding.UTF8, JsonMime),
                    Headers = {ETag = new EntityTagHeaderValue("\"123abc\"")}
                });
            var result = await _endpoint.ReadAllAsync();

            Mock.Expect(HttpMethod.Put, "http://localhost/endpoint/")
                .WithContent("[{\"Id\":5,\"Name\":\"test1\"},{\"Id\":6,\"Name\":\"test2\"}]")
                .WithHeaders("If-Match", "\"123abc\"")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent));

            await _endpoint.SetAllAsync(result);
        }

        [Test]
        public async Task TestCreateBulk()
        {
            Mock.Expect(new HttpMethod("PATCH"), "http://localhost/endpoint/")
                .WithContent("[{\"Id\":5,\"Name\":\"test1\"},{\"Id\":6,\"Name\":\"test2\"}]")
                .Respond(new HttpResponseMessage(HttpStatusCode.Accepted));

            await _endpoint.CreateAsync(new[] {new MockEntity(5, "test1"), new MockEntity(6, "test2")});
        }
    }
}