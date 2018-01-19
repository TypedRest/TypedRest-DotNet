using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Text;
using FluentAssertions;
using RichardSzalay.MockHttp;
using Xunit;

namespace TypedRest
{
    public class StreamEndpointTest : EndpointTestBase
    {
        private readonly IStreamEndpoint<MockEntity> _endpoint;

        public StreamEndpointTest() => _endpoint = new StreamEndpoint<MockEntity>(EntryEndpoint, "endpoint");

        [Fact]
        public void TestGetStream()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .WithHeaders("Range", "elements=0-")
                .Respond(HttpStatusCode.PartialContent,
                    new StringContent("[{\"id\":5,\"name\":\"test1\"},{\"id\":6,\"name\":\"test2\"}]", Encoding.UTF8, JsonMime)
                    {
                        Headers = {ContentRange = new ContentRangeHeaderValue(from: 0, to: 1) {Unit = "elements"}}
                    });

            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .WithHeaders("Range", "elements=2-")
                .Respond(HttpStatusCode.PartialContent,
                    new StringContent("[{\"id\":7,\"name\":\"test3\"}]", Encoding.UTF8, JsonMime)
                    {
                        Headers = {ContentRange = new ContentRangeHeaderValue(from: 2, to: 2, length: 3) {Unit = "elements"}}
                    });

            var stream = _endpoint.GetStream();
            stream.ToEnumerable().ToList().Should().Equal(
                new MockEntity(5, "test1"),
                new MockEntity(6, "test2"),
                new MockEntity(7, "test3"));
        }

        [Fact]
        public void TestGetStreamOffset()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .WithHeaders("Range", "elements=2-")
                .Respond(HttpStatusCode.PartialContent,
                    new StringContent("[{\"id\":7,\"name\":\"test3\"}]", Encoding.UTF8, JsonMime)
                    {
                        Headers = {ContentRange = new ContentRangeHeaderValue(from: 2, to: 2, length: 3) {Unit = "elements"}}
                    });

            var stream = _endpoint.GetStream(startIndex: 2);
            stream.ToEnumerable().ToList().Should().Equal(new MockEntity(7, "test3"));
        }

        [Fact]
        public void TestGetStreamTail()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .WithHeaders("Range", "elements=-1")
                .Respond(HttpStatusCode.PartialContent,
                    new StringContent("[{\"id\":7,\"name\":\"test3\"}]", Encoding.UTF8, JsonMime)
                    {
                        Headers = {ContentRange = new ContentRangeHeaderValue(from: 2, to: 2, length: 3) {Unit = "elements"}}
                    });

            var stream = _endpoint.GetStream(startIndex: -1);
            stream.ToEnumerable().ToList().Should().Equal(new MockEntity(7, "test3"));
        }
    }
}