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
    public class PagedCollectionEndpointTest : EndpointTestBase
    {
        private PagedCollectionEndpoint<MockEntity> _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new PagedCollectionEndpoint<MockEntity>(EntryEndpoint, "endpoint");
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
        public async Task TestReadRangeOffset()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint/")
                .WithHeaders("Range", "elements=1-")
                .Respond(HttpStatusCode.PartialContent,
                    new StringContent("[{\"Id\":6,\"Name\":\"test2\"}]", Encoding.UTF8, JsonMime)
                    {
                        Headers = {ContentRange = new ContentRangeHeaderValue(from: 1, to: 1, length: 2) {Unit = "elements"}}
                    });

            var response = await _endpoint.ReadRangeAsync(new RangeItemHeaderValue(from: 1, to: null));
            response.Elements.Should().Equal(new MockEntity {Id = 6, Name = "test2"});
            response.Range.Should().Be(new ContentRangeHeaderValue(from: 1, to: 1, length: 2) {Unit = "elements"});
        }

        [Test]
        public async Task TestReadRangeHead()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint/")
                .WithHeaders("Range", "elements=0-1")
                .Respond(HttpStatusCode.PartialContent,
                    new StringContent("[{\"Id\":5,\"Name\":\"test1\"},{\"Id\":6,\"Name\":\"test2\"}]", Encoding.UTF8, JsonMime)
                    {
                        Headers = {ContentRange = new ContentRangeHeaderValue(from: 0, to: 1, length: 2) {Unit = "elements"}}
                    });

            var response = await _endpoint.ReadRangeAsync(new RangeItemHeaderValue(from: 0, to: 1));
            response.Elements.Should().Equal(new MockEntity(5, "test1"), new MockEntity(6, "test2"));
            response.Range.Should().Be(new ContentRangeHeaderValue(from: 0, to: 1, length: 2) {Unit = "elements"});
        }

        [Test]
        public async Task TestReadRangeTail()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint/")
                .WithHeaders("Range", "elements=-1")
                .Respond(HttpStatusCode.PartialContent,
                    new StringContent("[{\"Id\":6,\"Name\":\"test2\"}]", Encoding.UTF8, JsonMime)
                    {
                        Headers = {ContentRange = new ContentRangeHeaderValue(from: 2, to: 2) {Unit = "elements"}}
                    });

            var response = await _endpoint.ReadRangeAsync(new RangeItemHeaderValue(from: null, to: 1));
            response.Elements.Should().Equal(new MockEntity(6, "test2"));
            response.Range.Should().Be(new ContentRangeHeaderValue(from: 2, to: 2) {Unit = "elements"});
        }

        [Test]
        public async Task TestException()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint/")
                .WithHeaders("Range", "elements=5-10")
                .Respond(HttpStatusCode.RequestedRangeNotSatisfiable, JsonMime, "{\"message\":\"test\"}");

            string exceptionMessage = null;
            try
            {
                await _endpoint.ReadRangeAsync(new RangeItemHeaderValue(from: 5, to: 10));
            }
            catch (InvalidOperationException ex)
            {
                exceptionMessage = ex.Message;
            }

            exceptionMessage.Should().Be("test");
        }
    }
}