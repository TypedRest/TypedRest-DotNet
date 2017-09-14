using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Text;
using FluentAssertions;
using RichardSzalay.MockHttp;
using Xunit;

namespace TypedRest
{
    public class PollingEndpointTest : EndpointTestBase
    {
        private readonly IPollingEndpoint<MockEntity> _endpoint;

        public PollingEndpointTest() => _endpoint = new PollingEndpoint<MockEntity>(EntryEndpoint, "endpoint", endCondition: x => x.Id == 3)
        {
            PollingInterval = TimeSpan.Zero
        };

        [Fact]
        public void TestGetStream()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(JsonMime, "{\"Id\":1,\"Name\":\"test\"}");
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(JsonMime, "{\"Id\":2,\"Name\":\"test\"}");
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage
                {
                    Content = new StringContent("{\"Id\":3,\"Name\":\"test\"}", Encoding.UTF8, JsonMime),
                    Headers = {RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(42))}
                });

            var stream = _endpoint.GetStream();
            stream.ToEnumerable().ToList().Should().Equal(
                new MockEntity(1, "test"),
                new MockEntity(2, "test"),
                new MockEntity(3, "test"));
            _endpoint.PollingInterval.Should().Be(TimeSpan.FromSeconds(42));
        }
    }
}