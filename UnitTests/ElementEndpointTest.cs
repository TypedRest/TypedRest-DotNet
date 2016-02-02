using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    [TestFixture]
    public class ElementEndpointTest : EndpointTestBase
    {
        private ElementEndpoint<MockEntity> _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new ElementEndpoint<MockEntity>(EntryEndpoint, "endpoint");
        }

        [Test]
        public async Task TestRead()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(JsonMime, "{\"Id\":5,\"Name\":\"test\"}");

            var result = await _endpoint.ReadAsync();
            result.Should().Be(new MockEntity(5, "test"));
        }

        [Test]
        public async Task TestReadCache()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("{\"Id\":5,\"Name\":\"test\"}", Encoding.UTF8, JsonMime),
                    Headers = {ETag = new EntityTagHeaderValue("\"123abc\"")}
                });
            var result = await _endpoint.ReadAsync();
            result.Should().Be(new MockEntity(5, "test"));

            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .WithHeaders("If-None-Match", "\"123abc\"")
                .Respond(HttpStatusCode.NotModified);
            result = await _endpoint.ReadAsync();
            result.Should().Be(new MockEntity(5, "test"));
        }

        [Test]
        public async Task TestUpdate()
        {
            Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
                .WithContent("{\"Id\":5,\"Name\":\"test\"}")
                .Respond(HttpStatusCode.NoContent);

            await _endpoint.UpdateAsync(new MockEntity(5, "test"));
        }

        [Test]
        public async Task TestUpdateEtag()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("{\"Id\":5,\"Name\":\"test\"}", Encoding.UTF8, JsonMime),
                    Headers = {ETag = new EntityTagHeaderValue("\"123abc\"")}
                });
            var result = await _endpoint.ReadAsync();

            Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
                .WithContent("{\"Id\":5,\"Name\":\"test\"}")
                .WithHeaders("If-Match", "\"123abc\"")
                .Respond(HttpStatusCode.NoContent);
            await _endpoint.UpdateAsync(result);
        }

        [Test]
        public async Task TestDelete()
        {
            Mock.Expect(HttpMethod.Delete, "http://localhost/endpoint")
                .Respond(HttpStatusCode.NoContent);

            await _endpoint.DeleteAsync();
        }
    }
}