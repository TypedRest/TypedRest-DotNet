using System;
using System.Net;
using System.Net.Http;
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
        public async Task TestSetAll()
        {
            Mock.Expect(HttpMethod.Put, "http://localhost/endpoint/")
                .WithContent("[{\"Id\":5,\"Name\":\"test1\"},{\"Id\":6,\"Name\":\"test2\"}]")
                .Respond(new HttpResponseMessage(HttpStatusCode.NoContent));

            await _endpoint.SetAllAsync(new[] {new MockEntity(5, "test1"), new MockEntity(6, "test2")});
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
        public async Task TestCreateBluk()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint/")
                .WithContent("[{\"Id\":5,\"Name\":\"test1\"},{\"Id\":6,\"Name\":\"test2\"}]")
                .Respond(new HttpResponseMessage(HttpStatusCode.Accepted));

            await _endpoint.CreateAsync(new[] {new MockEntity(5, "test1"), new MockEntity(6, "test2")});
        }

        [Test]
        public void TestGetByRelativeUri()
        {
            _endpoint[new Uri("1", UriKind.Relative)].Uri
                .Should().Be(new Uri(_endpoint.Uri, "1"));
        }

        [Test]
        public void TestGetByEntity()
        {
            _endpoint[new MockEntity(1, "test")].Uri
                .Should().Be(new Uri(_endpoint.Uri, "1"));
        }

        [Test]
        public async Task TestGetByEntityWithLinkHeader()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint/")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("[]", Encoding.UTF8, JsonMime),
                    Headers = {{"Link", "<children/{id}>; rel=child; templated=true"}}
                });

            await _endpoint.ReadAllAsync();

            _endpoint[new MockEntity(1, "test")].Uri.PathAndQuery
                .Should().Be("/endpoint/children/1");
        }
    }
}