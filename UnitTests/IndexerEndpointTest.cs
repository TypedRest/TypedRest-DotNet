using System;
using FluentAssertions;
using Xunit;

namespace TypedRest
{
    [Collection("Endpoint")]
    public class IndexerEndpointTest : EndpointTestBase
    {
        private readonly IndexerEndpoint<ElementEndpoint<MockEntity>> _endpoint;

        public IndexerEndpointTest()
        {
            _endpoint = new IndexerEndpoint<ElementEndpoint<MockEntity>>(EntryEndpoint, "endpoint");
        }

        [Fact]
        public void TestGetById()
            => _endpoint["1"].Uri.Should().Be(new Uri("http://localhost/endpoint/1"));
    }
}
