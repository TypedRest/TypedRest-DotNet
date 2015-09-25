using System;
using NUnit.Framework;

namespace TypedRest
{
    public abstract class RestEndpointTest
    {
        protected RestEntryPoint EntryPoint;

        [SetUp]
        public virtual void SetUp()
        {
            EntryPoint = new RestEntryPoint(new Uri("http://localhost:8089/"));
        }
    }
}