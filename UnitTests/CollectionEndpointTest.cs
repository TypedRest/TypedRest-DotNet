using System;
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
    public class CollectionEndpointTest : EndpointTestBase
    {
        private CollectionEndpoint<MockEntity> _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new CollectionEndpoint<MockEntity>(EntryEndpoint, "endpoint");
        }

        [Test]
        public async Task TestReadAll()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint/")
                .Respond(JsonMime, "[{\"Id\":5,\"Name\":\"test1\"},{\"Id\":6,\"Name\":\"test2\"}]");

            var result = await _endpoint.ReadAllAsync();
            result.Should().Equal(new MockEntity(5, "test1"), new MockEntity(6, "test2"));
        }

        [Test]
        public async Task TestReadAllCache()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint/")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("[{\"Id\":5,\"Name\":\"test1\"},{\"Id\":6,\"Name\":\"test2\"}]", Encoding.UTF8, JsonMime),
                    Headers = {ETag = new EntityTagHeaderValue("\"123abc\"")}
                });
            var result1 = await _endpoint.ReadAllAsync();
            result1.Should().Equal(new MockEntity(5, "test1"), new MockEntity(6, "test2"));

            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint/")
                .WithHeaders("If-None-Match", "\"123abc\"")
                .Respond(HttpStatusCode.NotModified);
            var result2 = await _endpoint.ReadAllAsync();
            result2.Should().Equal(new MockEntity(5, "test1"), new MockEntity(6, "test2"));

            result2.Should().NotBeSameAs(result1,
                because: "Cache responses, not deserialized objects");
        }

        [Test]
        public async Task TestCreate()
        {
            var location = new Uri("/endpoint/new", UriKind.Relative);
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint/")
                .WithContent("{\"Id\":5,\"Name\":\"test\"}")
                .Respond(new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Headers = {Location = location}
                });

            var element = await _endpoint.CreateAsync(new MockEntity(5, "test"));
            element.Uri.Should().Be(new Uri(EntryEndpoint.Uri, location));
        }

        [Test]
        public void TestGetByEntity()
        {
            _endpoint[new MockEntity(1, "test")].Uri
                .Should().Be(new Uri(_endpoint.Uri, "1"));
        }

        [Test]
        public void TestGetByEntityTemplate()
        {
            _endpoint[new MockEntity(1, "test")].Uri
                .Should().Be(new Uri(_endpoint.Uri, "1"));
        }

        [Test]
        public async Task TestGetByEntityWithLinkHeaderRelative()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint/")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("[]", Encoding.UTF8, JsonMime),
                    Headers = {{"Link", "<children/{id}>; rel=child; templated=true"}}
                });

            await _endpoint.ReadAllAsync();

            _endpoint[new MockEntity(1, "test")].Uri
                .Should().Be(new Uri("http://localhost/endpoint/children/1"));
        }

        [Test]
        public async Task TestGetByEntityWithLinkHeaderAbsolute()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint/")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("[]", Encoding.UTF8, JsonMime),
                    Headers = {{"Link", "<http://localhost/endpoint/children/{id}>; rel=child; templated=true"}}
                });

            await _endpoint.ReadAllAsync();

            _endpoint[new MockEntity(1, "test")].Uri
                .Should().Be(new Uri("http://localhost/endpoint/children/1"));
        }
    }
}