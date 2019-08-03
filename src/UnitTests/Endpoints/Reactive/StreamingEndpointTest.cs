using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using FluentAssertions;
using RichardSzalay.MockHttp;
using Xunit;

namespace TypedRest.Endpoints.Reactive
{
    [Collection("Endpoint")]
    public class StreamingEndpointTest : EndpointTestBase
    {
        private readonly IStreamingEndpoint<MockEntity> _endpoint;

        public StreamingEndpointTest()
        {
            _endpoint = new StreamingEndpoint<MockEntity>(EntryEndpoint, "endpoint");
        }

        [Fact]
        public void TestGetObservable()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(JsonMime, "{\"id\":5,\"name\":\"test1\"}\n{\"id\":6,\"name\":\"test2\"}\n{\"id\":7,\"name\":\"test3\"}");

            var observable = _endpoint.GetObservable();
            observable.ToEnumerable().ToList().Should().Equal(
                new MockEntity(5, "test1"),
                new MockEntity(6, "test2"),
                new MockEntity(7, "test3"));
        }

        [Fact]
        public void TestErrorHandling()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.Conflict));

            var observable = _endpoint.GetObservable();
            observable.Invoking(x =>
            {
                var _ = x.ToEnumerable().ToList();
            }).Should().Throw<InvalidOperationException>();
        }
    }
}
