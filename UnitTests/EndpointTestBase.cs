using System;
using System.Net.Http;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    public abstract class EndpointTestBase
    {
        public const string JsonMime = "application/json";

        protected IEndpoint EntryEndpoint;
        protected MockHttpMessageHandler Mock;

        [SetUp]
        public virtual void SetUp()
        {
            EntryEndpoint = new MockEntryEndpoint(Mock = new MockHttpMessageHandler());
        }

        [TearDown]
        public void TearDown()
        {
            Mock.VerifyNoOutstandingExpectation();
        }

        private class MockEntryEndpoint : EndpointBase
        {
            public MockEntryEndpoint(MockHttpMessageHandler mockHttp)
                : base(new HttpClient(mockHttp), new Uri("http://localhost/"))
            {
            }
        }
    }
}