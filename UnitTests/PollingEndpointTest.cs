using System;
using System.Net.Http;
using System.Reactive.Linq;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    [TestFixture]
    public class PollingEndpointTest : EndpointTestBase
    {
        private IPollingEndpoint<MockEntity> _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new PollingEndpoint<MockEntity>(EntryEndpoint, "endpoint");
        }

        [Test]
        public void TestGetStream()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(JsonMime, "{\"Id\":1,\"Name\":\"test\"}");
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(JsonMime, "{\"Id\":2,\"Name\":\"test\"}");
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(JsonMime, "{\"Id\":3,\"Name\":\"test\"}");

            var endpoint = _endpoint.GetStream(TimeSpan.Zero, endCondition: x => x.Id == 3);
            endpoint.ToEnumerable().Should().Equal(
                new MockEntity(1, "test"),
                new MockEntity(2, "test"),
                new MockEntity(3, "test"));
        }
    }
}