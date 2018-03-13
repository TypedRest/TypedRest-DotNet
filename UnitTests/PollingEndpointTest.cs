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
    [Collection("Endpoint")]
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
                .Respond(JsonMime, "{\"id\":1,\"name\":\"test\"}");
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(JsonMime, "{\"id\":2,\"name\":\"test\"}");
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage
                {
                    Content = new StringContent("{\"id\":3,\"name\":\"test\"}", Encoding.UTF8, JsonMime),
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