using System;
using FluentAssertions;
using Xunit;

namespace TypedRest
{
    [Collection("Endpoint")]
    public class IndexerEndpointTest : EndpointTestBase
    {
        [Fact]
        public void TestGetById()
        {
            var endpoint = new IndexerEndpoint<ElementEndpoint<MockEntity>>(EntryEndpoint, "endpoint");

            endpoint["1"].Uri.Should().Be(new Uri("http://localhost/endpoint/1"));
        }

        [Fact]
        public void RejectsAbstractTypes()
        {
            Func<IndexerEndpoint<EndpointBase>> ctor = () => new IndexerEndpoint<EndpointBase>(EntryEndpoint, "endpoint");
            ctor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AcceptsDerivedTypes()
        {
            Func<IndexerEndpoint<EndpointBase>> ctor = () => new IndexerEndpoint<EndpointBase>(EntryEndpoint, "endpoint", instanceType: typeof(ElementEndpoint<MockEntity>));
            ctor.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void RejectsNonAssignableTypes()
        {
            Func<IndexerEndpoint<ElementEndpoint<MockEntity>>> ctor = () => new IndexerEndpoint<ElementEndpoint<MockEntity>>(EntryEndpoint, "endpoint", instanceType: typeof(ElementEndpoint<string>));
            ctor.Should().Throw<ArgumentException>();
        }
    }
}
