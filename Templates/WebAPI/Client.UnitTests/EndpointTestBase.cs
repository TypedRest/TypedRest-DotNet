using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using TypedRest;

namespace XProjectNamespaceX.Client
{
    public abstract class EndpointTestBase
    {
        public const string JsonMime = "application/json";

        protected IEndpoint EntryEndpoint;
        protected MockHttpMessageHandler Mock;

        [SetUp]
        public virtual void SetUp()
        {
            EntryEndpoint = new MockEntryEndpoint(Mock = new SubMockHttpMessageHandler());
        }

        [TearDown]
        public void TearDown()
        {
            Mock.VerifyNoOutstandingExpectation();
        }

        private class MockEntryEndpoint : EndpointBase
        {
            public MockEntryEndpoint(HttpMessageHandler messageHandler)
                : base(new HttpClient(messageHandler), new Uri("http://localhost/"))
            {
            }
        }

        private class SubMockHttpMessageHandler : MockHttpMessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return await base.SendAsync(request, cancellationToken);
            }
        }
    }
}
