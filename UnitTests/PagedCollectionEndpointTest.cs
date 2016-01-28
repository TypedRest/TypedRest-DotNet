using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace TypedRest
{
    [TestFixture, Ignore("Server mock not implemented yet")]
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
            //stubFor(get(urlEqualTo("/endpoint/"))
            //    .withHeader("Accept", equalTo(jsonMime))
            //    .willReturn(aResponse()
            //        .withStatus(200)
            //        .withHeader("Content-Type", jsonMime)
            //        .withBody("[{\"id\":5,\"name\":\"test1\"},{\"id\":6,\"name\":\"test2\"}]")));

            var result = await _endpoint.ReadAllAsync();
            result.Should().Equal(new MockEntity(5, "test1"), new MockEntity(6, "test2"));
        }

        [Test]
        public async Task TestReadRangeOffset()
        {
            //stubFor(get(urlEqualTo("/endpoint"))
            //        .withHeader("Accept", equalTo(jsonMime))
            //        .withHeader("Range", equalTo("elements=1-"))
            //        .willReturn(aResponse()
            //                .withStatus(SC_PARTIAL_CONTENT)
            //                .withHeader("Content-Type", jsonMime)
            //                .withHeader("Content-Range", "elements 1-1/2")
            //                .withBody("[{\"id\":6,\"name\":\"test2\"}]")));

            var response = await _endpoint.ReadRangeAsync(new RangeItemHeaderValue(from: 1, to: null));
            response.Elements.Should().Equal(new MockEntity {Id = 6, Name = "test2"});
            response.Range.Should().Be(new ContentRangeHeaderValue(from: 1, to: 1, length: 2));
        }

        [Test]
        public async Task TestReadRangeHead()
        {
            //stubFor(get(urlEqualTo("/endpoint"))
            //        .withHeader("Accept", equalTo(jsonMime))
            //        .withHeader("Range", equalTo("elements=0-1"))
            //        .willReturn(aResponse()
            //                .withStatus(SC_PARTIAL_CONTENT)
            //                .withHeader("Content-Type", jsonMime)
            //                .withHeader("Content-Range", "elements 0-1/2")
            //                .withBody("[{\"id\":5,\"name\":\"test1\"},{\"id\":6,\"name\":\"test2\"}]")));

            var response = await _endpoint.ReadRangeAsync(new RangeItemHeaderValue(from: 1, to: 1));
            response.Elements.Should().Equal(new MockEntity(5, "test1"), new MockEntity(6, "test2"));
            response.Range.Should().Be(new ContentRangeHeaderValue(from: 0, to: 1, length: 2));
        }

        [Test]
        public async Task TestReadRangeTail()
        {
            //stubFor(get(urlEqualTo("/endpoint"))
            //        .withHeader("Accept", equalTo(jsonMime))
            //        .withHeader("Range", equalTo("elements=-1"))
            //        .willReturn(aResponse()
            //                .withStatus(SC_PARTIAL_CONTENT)
            //                .withHeader("Content-Type", jsonMime)
            //                .withHeader("Content-Range", "elements 2-2/*")
            //                .withBody("[{\"id\":6,\"name\":\"test2\"}]")));

            var response = await _endpoint.ReadRangeAsync(new RangeItemHeaderValue(from: null, to: 1));
            response.Elements.Should().Equal(new MockEntity(6, "test2"));
            response.Range.Should().Be(new ContentRangeHeaderValue(from: 2, to: 2));
        }

        [Test]
        public async Task TestException()
        {
            //stubFor(get(urlEqualTo("/endpoint"))
            //        .withHeader("Range", equalTo("elements=5-10"))
            //        .willReturn(aResponse()
            //                .withStatus(SC_REQUESTED_RANGE_NOT_SATISFIABLE)
            //                .withHeader("Content-Type", jsonMime)
            //                .withBody("{\"message\":\"test\"}")));

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