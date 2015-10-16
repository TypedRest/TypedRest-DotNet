using System.Collections.Generic;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace TypedRest
{
    [TestFixture, Ignore("Server mock not implemented yet")]
    public class StreamEndpointTest : EndpointTestBase
    {
        private IStreamEndpoint<MockEntity> _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new StreamEndpoint<MockEntity>(EntryEndpoint, "endpoint");
        }

        [Test]
        public void TestGetStream()
        {
            //stubFor(get(urlEqualTo("/endpoint"))
            //        .withHeader("Accept", equalTo(jsonMime))
            //        .withHeader("Range", equalTo("elements=0-"))
            //        .willReturn(aResponse()
            //                .withStatus(SC_PARTIAL_CONTENT)
            //                .withHeader("Content-Type", jsonMime)
            //                .withHeader("Content-Range", "elements 0-1/*")
            //                .withBody("[{\"id\":5,\"name\":\"test1\"},{\"id\":6,\"name\":\"test2\"}]")));

            //stubFor(get(urlEqualTo("/endpoint"))
            //        .withHeader("Accept", equalTo(jsonMime))
            //        .withHeader("Range", equalTo("elements=2-"))
            //        .willReturn(aResponse()
            //                .withStatus(SC_PARTIAL_CONTENT)
            //                .withHeader("Content-Type", jsonMime)
            //                .withHeader("Content-Range", "elements 2-2/3")
            //                .withBody("[{\"id\":7,\"name\":\"test3\"}]")));

            var stream = _endpoint.GetStream();
            stream.AssertEqual(new List<MockEntity>
            {
                new MockEntity(5, "test1"),
                new MockEntity(6, "test2"),
                new MockEntity(7, "test3")
            }.ToObservable());
        }

        [Test]
        public void TestGetStreamOffset()
        {
            //stubFor(get(urlEqualTo("/endpoint"))
            //        .withHeader("Accept", equalTo(jsonMime))
            //        .withHeader("Range", equalTo("elements=2-"))
            //        .willReturn(aResponse()
            //                .withStatus(SC_PARTIAL_CONTENT)
            //                .withHeader("Content-Type", jsonMime)
            //                .withHeader("Content-Range", "elements 2-2/3")
            //                .withBody("[{\"id\":7,\"name\":\"test3\"}]")));

            var stream = _endpoint.GetStream(startIndex: 2);
            stream.AssertEqual(new List<MockEntity> {new MockEntity(7, "test3")}.ToObservable());
        }

        [Test]
        public void TestGetStreamOffsetTail()
        {
            //stubFor(get(urlEqualTo("/endpoint"))
            //        .withHeader("Accept", equalTo(jsonMime))
            //        .withHeader("Range", equalTo("elements=-1"))
            //        .willReturn(aResponse()
            //                .withStatus(SC_PARTIAL_CONTENT)
            //                .withHeader("Content-Type", jsonMime)
            //                .withHeader("Content-Range", "elements 2-2/3")
            //                .withBody("[{\"id\":7,\"name\":\"test3\"}]")));

            var stream = _endpoint.GetStream(startIndex: -1);
            stream.AssertEqual(new List<MockEntity> {new MockEntity(7, "test3")}.ToObservable());
        }
    }
}