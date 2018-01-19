using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    public abstract class EndpointTestBase : IDisposable
    {
        public const string JsonMime = "application/json";

        protected readonly MockHttpMessageHandler Mock = new SubMockHttpMessageHandler();
        protected readonly IEndpoint EntryEndpoint;

        protected EndpointTestBase() => EntryEndpoint = new MockEntryEndpoint(Mock);

        public void Dispose() => Mock.VerifyNoOutstandingExpectation();

        private class MockEntryEndpoint : EntryEndpoint
        {
            public MockEntryEndpoint(HttpMessageHandler messageHandler)
                : base(new Uri("http://localhost/"), new HttpClient(messageHandler), new JsonMediaTypeFormatter())
            {}
        }

        private class SubMockHttpMessageHandler : MockHttpMessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => await base.SendAsync(request, cancellationToken);
        }
    }
}