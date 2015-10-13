using System;
using NUnit.Framework;

namespace TypedRest
{
    public abstract class EndpointTestBase
    {
        protected EntryEndpoint EntryEndpoint;

        [SetUp]
        public virtual void SetUp()
        {
            EntryEndpoint = new EntryEndpoint(new Uri("http://localhost:8089/"));
        }
    }
}